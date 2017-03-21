using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Windows.Forms;

//using DSShared.Windows;


//#define SaveDLL

namespace MapView
{
	public class PropertyForm
		:
		Form
	{
		private CustomPropertyGrid propertyGrid;


		public PropertyForm(string typeLabel, Settings settings)
		{
			InitializeComponent();
//			var ri = new RegistryInfo(this, "OptionsForm");

			propertyGrid.TypeLabel = typeLabel;
			propertyGrid.SetSettings(settings);
		}


		#region Windows Form Designer generated code

		private void InitializeComponent()
		{
			this.propertyGrid = new global::MapView.CustomPropertyGrid();
			this.SuspendLayout();
			// 
			// propertyGrid
			// 
			this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.propertyGrid.LineColor = System.Drawing.SystemColors.ScrollBar;
			this.propertyGrid.Location = new System.Drawing.Point(0, 0);
			this.propertyGrid.Name = "propertyGrid";
			this.propertyGrid.Size = new System.Drawing.Size(242, 325);
			this.propertyGrid.TabIndex = 0;
			this.propertyGrid.TypeLabel = "DefaultType";
			// 
			// PropertyForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(242, 325);
			this.Controls.Add(this.propertyGrid);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Name = "PropertyForm";
			this.Text = "Custom PropertyGrid";
			this.ResumeLayout(false);

		}
		#endregion
	}


	public class CustomPropertyGrid
		:
		PropertyGrid
	{
		private string _typeLabel = "DefaultType";
		private Settings _settings;

		private bool _instantUpdate = true;

		private Hashtable _typeHash;
		private static Hashtable _hashTypes = new Hashtable();


		public CustomPropertyGrid()
		{
			initTypes();
		}


		/// <summary>
		/// Initialize a private hashtable with type-opCode pairs so i dont have
		/// to write a long if/else statement when outputting msil
		/// </summary>
		private void initTypes()
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

		[Description("Name of the type that will be internally created")]
		public string TypeLabel
		{
			get { return _typeLabel; }
			set { _typeLabel = value; }
		}

		[DefaultValue(true)] // NOTE: This doesn't affect the default value; it is used only by the designer.
		[Description("If true, the Setting.Update() event will be called when a property changes")]
		public bool InstantUpdate
		{
			get { return _instantUpdate; }
			set { _instantUpdate = value; }
		}

		protected override void OnPropertyValueChanged(PropertyValueChangedEventArgs e)
		{
			base.OnPropertyValueChanged(e);

			((Setting)_settings[e.ChangedItem.Label]).Value = e.ChangedItem.Value;

			if (_instantUpdate)
				((Setting)_settings[e.ChangedItem.Label]).FireUpdate(
																e.ChangedItem.Label,
																e.ChangedItem.Value);
		}

		/// <summary>
		/// I always wanted a function like this.
		/// </summary>
		/// <param name="settings">the settings to set</param>
		public void SetSettings(Settings settings)
		{
			_settings = settings;

			// Reflection.Emit code below copied and modified
			// http://longhorn.msdn.microsoft.com/lhsdk/ref/ns/system.reflection.emit/c/propertybuilder/propertybuilder.aspx

			if (_hashTypes[_typeLabel] == null)
			{
				var myDomain = Thread.GetDomain();
				var myAsmName = new AssemblyName();
				myAsmName.Name = "TempAssembly";

				// Only save the custom-type dll while debugging
#if SaveDLL && DEBUG
				AssemblyBuilder assemblyBuilder = myDomain.DefineDynamicAssembly(
																			myAsmName,
																			AssemblyBuilderAccess.RunAndSave);
				ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(
																			"TempModule",
																			"Test.dll");
#else
				AssemblyBuilder assemblyBuilder = myDomain.DefineDynamicAssembly(
																			myAsmName,
																			AssemblyBuilderAccess.Run);
				ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("TempModule");
#endif

				// create type
				TypeBuilder newType = moduleBuilder.DefineType(_typeLabel, TypeAttributes.Public);

				// create the hashtable used to store property values
				FieldBuilder hashField = newType.DefineField(
														"table",
														typeof(Hashtable),
														FieldAttributes.Private);
				createHashMethod(
							newType.DefineProperty(
												"Hash",
												PropertyAttributes.None,
												typeof(Hashtable),
												new Type[]{}),
							newType,
							hashField);

				foreach (string key in _settings.Keys)
					emitProperty(
								newType,
								hashField,
								_settings[key],
								key);

				_hashTypes[_typeLabel] = newType.CreateType();
			}

			var table = new Hashtable();
			foreach (string key in _settings.Keys)
				table[key] = _settings[key].Value;

#if SaveDLL && DEBUG
			assemblyBuilder.Save("Test.dll");
#endif
			var custType = (Type)_hashTypes[_typeLabel];
			var ctorInfo = custType.GetConstructor(new Type[]{});
			object obj = ctorInfo.Invoke(new Object[]{});

			// set the object's hashtable - in the future i would like to do this in the emitted object's constructor
			var propInfo = custType.GetProperty("Hash");
			propInfo.SetValue(obj, table, null);

			SelectedObject = obj;
		}

		private static void createHashMethod(
				PropertyBuilder propBuild,
				TypeBuilder typeBuild,
				FieldInfo hash)
		{
			// First, define the behavior of the "get" property for Hash as a method.
			var typeHashGet = typeBuild.DefineMethod(
												"GetHash",
												MethodAttributes.Public,
												typeof(Hashtable),
												new Type[]{});
			ILGenerator ilg = typeHashGet.GetILGenerator();
			ilg.Emit(OpCodes.Ldarg_0);
			ilg.Emit(OpCodes.Ldfld, hash);
			ilg.Emit(OpCodes.Ret);

			// Now, define the behavior of the "set" property for Hash.
			var typeHashSet = typeBuild.DefineMethod(
												"SetHash",
												MethodAttributes.Public,
												null,
												new Type[]{ typeof(Hashtable) });

			ilg = typeHashSet.GetILGenerator();
			ilg.Emit(OpCodes.Ldarg_0);
			ilg.Emit(OpCodes.Ldarg_1);
			ilg.Emit(OpCodes.Stfld, hash);
			ilg.Emit(OpCodes.Ret);

			// map the two methods created above to their property
			propBuild.SetGetMethod(typeHashGet);
			propBuild.SetSetMethod(typeHashSet);

			// add the [Browsable(false)] property to the Hash property so it doesn't show up on the property list
			var ci = typeof(BrowsableAttribute).GetConstructor(new Type[]{ typeof(bool) });
			var cab = new CustomAttributeBuilder(ci,new object[]{ false });
			propBuild.SetCustomAttribute(cab);
		}

		/// <summary>
		/// Emits a generic get/set property in which the result returned resides
		/// in a hashtable whose key is the name of the property
		/// </summary>
		/// <param name="builder"></param>
		/// <param name="hash"></param>
		/// <param name="setting"></param>
		/// <param name="name"></param>
		private void emitProperty(
				TypeBuilder builder,
				FieldInfo hash,
				Setting setting,
				string name)
		{
			// to figure out what opcodes to emit, i would compile a small class
			// having the functionality i wanted, and view it with ildasm.
			// peverify is also kinda nice to use to see what errors there are.

			var pb = builder.DefineProperty(
										name,
										PropertyAttributes.None,
										setting.Value.GetType(),
										new Type[]{});
			var objType = setting.Value.GetType();
			var getMethod = builder.DefineMethod(
											"get_" + name,
											MethodAttributes.Public,
											objType,
											new Type[]{});
			var ilg = getMethod.GetILGenerator();
			ilg.DeclareLocal(objType);
			ilg.Emit(OpCodes.Ldarg_0);
			ilg.Emit(OpCodes.Ldfld,hash);
			ilg.Emit(OpCodes.Ldstr,name);
			ilg.EmitCall(
					OpCodes.Callvirt,
					typeof(Hashtable).GetMethod("get_Item"),
					null);

			if (objType.IsValueType)
			{
				ilg.Emit(OpCodes.Unbox,objType);
				if (_typeHash[objType] != null)
					ilg.Emit((OpCode)_typeHash[objType]);
				else
					ilg.Emit(OpCodes.Ldobj, objType);
			}
			else
				ilg.Emit(OpCodes.Castclass, objType);

			ilg.Emit(OpCodes.Stloc_0);
			ilg.Emit(OpCodes.Br_S, (byte)0);
			ilg.Emit(OpCodes.Ldloc_0);
			ilg.Emit(OpCodes.Ret);

			var setMethod = builder.DefineMethod(
											"set_" + name,
											MethodAttributes.Public,
											null,
											new Type[]{ objType });
			ilg = setMethod.GetILGenerator();
			ilg.Emit(OpCodes.Ldarg_0);
			ilg.Emit(OpCodes.Ldfld,hash);
			ilg.Emit(OpCodes.Ldstr,name);
			ilg.Emit(OpCodes.Ldarg_1);

			if (objType.IsValueType)
				ilg.Emit(OpCodes.Box, objType);

			ilg.EmitCall(
					OpCodes.Callvirt,
					typeof(Hashtable).GetMethod("set_Item"),
					null);
			ilg.Emit(OpCodes.Ret);

			pb.SetGetMethod(getMethod);
			pb.SetSetMethod(setMethod);

			if (setting.Description != null)
			{
				var ci = typeof(DescriptionAttribute).GetConstructor(new Type[]{ typeof(string) });
				var cab = new CustomAttributeBuilder(ci, new object[]{ setting.Description });
				pb.SetCustomAttribute(cab);
			}

			if (setting.Category != null)
			{
				var ci = typeof(CategoryAttribute).GetConstructor(new Type[]{ typeof(string) });
				var cab = new CustomAttributeBuilder(ci, new object[]{ setting.Category });
				pb.SetCustomAttribute(cab);
			}
		}
	}
}
