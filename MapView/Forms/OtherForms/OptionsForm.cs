using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Windows.Forms;

using DSShared.Windows;


//#define SaveDLL

namespace MapView
{
	internal sealed class OptionsForm
		:
			Form
	{
		private OptionsPropertyGrid _propertyGrid;


		internal OptionsForm(string typeLabel, Options options)
		{
			InitializeComponent();

			var regInfo = new RegistryInfo(RegistryInfo.Options, this); // subscribe to Load and Closing events.
			regInfo.RegisterProperties();

			_propertyGrid.TypeLabel = typeLabel;
			_propertyGrid.SetOptions(options);
		}


		#region Windows Form Designer generated code

		private void InitializeComponent()
		{
			this._propertyGrid = new MapView.OptionsPropertyGrid();
			this.SuspendLayout();
			// 
			// propertyGrid
			// 
			this._propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			this._propertyGrid.LineColor = System.Drawing.SystemColors.ScrollBar;
			this._propertyGrid.Location = new System.Drawing.Point(0, 0);
			this._propertyGrid.Name = "propertyGrid";
			this._propertyGrid.Size = new System.Drawing.Size(592, 374);
			this._propertyGrid.TabIndex = 0;
			// 
			// OptionsForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(592, 374);
			this.Controls.Add(this._propertyGrid);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.MinimumSize = new System.Drawing.Size(500, 300);
			this.Name = "OptionsForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Custom PropertyGrid";
			this.ResumeLayout(false);
		}
		#endregion
	}


	internal sealed class OptionsPropertyGrid
		:
			PropertyGrid
	{
		private string _typeLabel = "DefaultType";
		[Description("Name of the type that will be internally created.")]
		internal string TypeLabel
		{
			set { _typeLabel = value; }
		}

		private Options _options;

//		private bool _instantUpdate = true;

		private Hashtable _typeHash;
		private static Hashtable _hashTypes = new Hashtable();


		internal OptionsPropertyGrid()
		{
			InitTypes();
		}


		/// <summary>
		/// Initialize a private hashtable with type-opCode pairs so i dont have
		/// to write a long if/else statement when outputting MSIL.
		/// </summary>
		private void InitTypes()
		{
			_typeHash = new Hashtable();

			_typeHash[typeof(sbyte)]  = OpCodes.Ldind_I1;
			_typeHash[typeof(byte)]   = OpCodes.Ldind_U1;
			_typeHash[typeof(char)]   = OpCodes.Ldind_U2;
			_typeHash[typeof(short)]  = OpCodes.Ldind_I2;
			_typeHash[typeof(ushort)] = OpCodes.Ldind_U2;
			_typeHash[typeof(int)]    = OpCodes.Ldind_I4;
			_typeHash[typeof(uint)]   = OpCodes.Ldind_U4;
			_typeHash[typeof(long)]   = OpCodes.Ldind_I8;
			_typeHash[typeof(ulong)]  = OpCodes.Ldind_I8;
			_typeHash[typeof(bool)]   = OpCodes.Ldind_I1;
			_typeHash[typeof(double)] = OpCodes.Ldind_R8;
			_typeHash[typeof(float)]  = OpCodes.Ldind_R4;
		}

/*		[DefaultValue(true)]
		[Description("If true the Option.Update() event will be called when a property changes.")]
		public bool InstantUpdate
		{
			get { return _instantUpdate; }
			set { _instantUpdate = value; }
		} */

		// FxCop CA2123:OverrideLinkDemandsShouldBeIdenticalToBase
		[System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.LinkDemand, Name = "FullTrust")]
		protected override void OnPropertyValueChanged(PropertyValueChangedEventArgs e)
		{
			base.OnPropertyValueChanged(e);

			((ViewerOption)_options[e.ChangedItem.Label]).Value = e.ChangedItem.Value;

//			if (_instantUpdate)
			((ViewerOption)_options[e.ChangedItem.Label]).doUpdate(
															e.ChangedItem.Label,
															e.ChangedItem.Value);
		}

		/// <summary>
		/// Does some complicated stuff.
		/// </summary>
		/// <param name="options">the options to set</param>
		internal void SetOptions(Options options)
		{
			_options = options;

			// Reflection.Emit code below copied and modified
			// http://longhorn.msdn.microsoft.com/lhsdk/ref/ns/system.reflection.emit/c/propertybuilder/propertybuilder.aspx

			if (_hashTypes[_typeLabel] == null)
			{
				var ad = Thread.GetDomain();
				var an = new AssemblyName();
				an.Name = "TempAssembly";

				// Only save the custom-type dll while debugging
#if SaveDLL && DEBUG
				AssemblyBuilder assemblyBuilder = ad.DefineDynamicAssembly(
																		an,
																		AssemblyBuilderAccess.RunAndSave);
				ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(
																		"TempModule",
																		"Test.dll");
#else
				AssemblyBuilder assemblyBuilder = ad.DefineDynamicAssembly(
																		an,
																		AssemblyBuilderAccess.Run);
				ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("TempModule");
#endif

				// create type
				TypeBuilder typeBuilder = moduleBuilder.DefineType(_typeLabel, TypeAttributes.Public);

				// create the hashtable used to store property values
				FieldBuilder fieldBuilder = typeBuilder.DefineField(
																"table",
																typeof(Hashtable),
																FieldAttributes.Private);
				CreateHashMethod(
							typeBuilder.DefineProperty(
													"Hash",
													PropertyAttributes.None,
													typeof(Hashtable),
													new Type[]{}),
							typeBuilder,
							fieldBuilder);

				foreach (string key in _options.Keys)
					EmitProperty(
							typeBuilder,
							fieldBuilder,
							_options[key],
							key);

				_hashTypes[_typeLabel] = typeBuilder.CreateType();
			}

			var table = new Hashtable();
			foreach (string key in _options.Keys)
				table[key] = _options[key].Value;

#if SaveDLL && DEBUG
			assemblyBuilder.Save("Test.dll");
#endif
			var type = (Type)_hashTypes[_typeLabel];
			var ctorInfo = type.GetConstructor(new Type[]{});
			object obj = ctorInfo.Invoke(new Object[]{});

			var propInfo = type.GetProperty("Hash");	// set the object's hashtable
			propInfo.SetValue(obj, table, null);		// in the future i would like to do this in the emitted object's constructor

			SelectedObject = obj;
		}

		private static void CreateHashMethod(
				PropertyBuilder propBuilder,
				TypeBuilder typeBuilder,
				FieldInfo fieldInfo)
		{
			var methodGetter = typeBuilder.DefineMethod( // first define the behavior of the "get" property for Hash as a method
													"GetHash",
													MethodAttributes.Public,
													typeof(Hashtable),
													new Type[]{});
			ILGenerator generator = methodGetter.GetILGenerator();
			generator.Emit(OpCodes.Ldarg_0);
			generator.Emit(OpCodes.Ldfld, fieldInfo);
			generator.Emit(OpCodes.Ret);

			var methodSetter = typeBuilder.DefineMethod( // now define the behavior of the "set" property for Hash
													"SetHash",
													MethodAttributes.Public,
													null,
													new []{ typeof(Hashtable) });

			generator = methodSetter.GetILGenerator();
			generator.Emit(OpCodes.Ldarg_0);
			generator.Emit(OpCodes.Ldarg_1);
			generator.Emit(OpCodes.Stfld, fieldInfo);
			generator.Emit(OpCodes.Ret);

			// map the two methods created above to their property
			propBuilder.SetGetMethod(methodGetter);
			propBuilder.SetSetMethod(methodSetter);

			// add the [Browsable(false)] property to the Hash property so it doesn't show up on the property list
			var ctorInfo = typeof(BrowsableAttribute).GetConstructor(new []{ typeof(bool) });
			var attributeBuilder = new CustomAttributeBuilder(ctorInfo, new object[]{ false });
			propBuilder.SetCustomAttribute(attributeBuilder);
		}

		/// <summary>
		/// Emits a generic get/set property in which the result returned resides
		/// in a hashtable whose key is the name of the property.
		/// </summary>
		/// <param name="typeBuilder"></param>
		/// <param name="fieldInfo"></param>
		/// <param name="option"></param>
		/// <param name="name"></param>
		private void EmitProperty(
				TypeBuilder typeBuilder,
				FieldInfo fieldInfo,
				ViewerOption option,
				string name)
		{
			// to figure out what opcodes to emit, i would compile a small class
			// having the functionality i wanted, and view it with ildasm.
			// peverify is also kinda nice to use to see what errors there are.

			var propertyBuilder = typeBuilder.DefineProperty(
														name,
														PropertyAttributes.None,
														option.Value.GetType(),
														new Type[]{});
			var objType = option.Value.GetType();
			var methodGetter = typeBuilder.DefineMethod(
													"get_" + name,
													MethodAttributes.Public,
													objType,
													new Type[]{});
			var generator = methodGetter.GetILGenerator();
			generator.DeclareLocal(objType);
			generator.Emit(OpCodes.Ldarg_0);
			generator.Emit(OpCodes.Ldfld, fieldInfo);
			generator.Emit(OpCodes.Ldstr, name);
			generator.EmitCall(
							OpCodes.Callvirt,
							typeof(Hashtable).GetMethod("get_Item"),
							null);

			if (objType.IsValueType)
			{
				generator.Emit(OpCodes.Unbox, objType);
				if (_typeHash[objType] != null)
					generator.Emit((OpCode)_typeHash[objType]);
				else
					generator.Emit(OpCodes.Ldobj, objType);
			}
			else
				generator.Emit(OpCodes.Castclass, objType);

			generator.Emit(OpCodes.Stloc_0);
			generator.Emit(OpCodes.Br_S, (byte)0);
			generator.Emit(OpCodes.Ldloc_0);
			generator.Emit(OpCodes.Ret);

			var methodSetter = typeBuilder.DefineMethod(
													"set_" + name,
													MethodAttributes.Public,
													null,
													new []{ objType });
			generator = methodSetter.GetILGenerator();
			generator.Emit(OpCodes.Ldarg_0);
			generator.Emit(OpCodes.Ldfld, fieldInfo);
			generator.Emit(OpCodes.Ldstr, name);
			generator.Emit(OpCodes.Ldarg_1);

			if (objType.IsValueType)
				generator.Emit(OpCodes.Box, objType);

			generator.EmitCall(
							OpCodes.Callvirt,
							typeof(Hashtable).GetMethod("set_Item"),
							null);
			generator.Emit(OpCodes.Ret);

			propertyBuilder.SetGetMethod(methodGetter);
			propertyBuilder.SetSetMethod(methodSetter);

			if (option.Description != null)
			{
				var ctorInfo = typeof(DescriptionAttribute).GetConstructor(new []{ typeof(string) });
				var attributeBuilder = new CustomAttributeBuilder(ctorInfo, new object[]{ option.Description });
				propertyBuilder.SetCustomAttribute(attributeBuilder);
			}

			if (option.Category != null)
			{
				var ctorInfo = typeof(CategoryAttribute).GetConstructor(new []{ typeof(string) });
				var attributeBuilder = new CustomAttributeBuilder(ctorInfo, new object[]{ option.Category });
				propertyBuilder.SetCustomAttribute(attributeBuilder);
			}
		}
	}
}
