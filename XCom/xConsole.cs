using System;

using XCom;


namespace XCom
{
	public delegate void BufferChangedDelegate(Node current);


	public class xConsole
	{
		private static Node _curLine = null;
		private static int _qtyNodes;

		public static event BufferChangedDelegate BufferChanged;


		private xConsole()
		{}


		public static Node CurrNode
		{
			get { return _curLine; }
		}

		public static int NumNodes
		{
			get { return _qtyNodes; }
		}

		private static void makeNodes(int qtyLines)
		{
			if (_curLine == null)
			{
				_curLine = new Node(String.Empty);
				_curLine._next = new Node(String.Empty);

				Node curNode = _curLine._next;
				Node lastNode = _curLine;
				curNode._last = lastNode;
				for (int i = 2; i < qtyLines; i++)
				{
					curNode._next = new Node(String.Empty);
					curNode = curNode._next;
					curNode._last = lastNode._next;
					lastNode = lastNode._next;
				}

				curNode._next = _curLine;
				_curLine._last = curNode;
			}
			else
			{
				if (qtyLines > _qtyNodes)
				{
					Node curNode = _curLine;
					Node lastNode = _curLine._last;

					for (int i = 0; i < qtyLines - _qtyNodes; i++)
					{
						var node = new Node(String.Empty);
						node._next = curNode;
						node._last = lastNode;
						lastNode._next = node;
						curNode._last = node;
						lastNode = node;
					}
				}
				else
				{
					for (int i = 0; i < _qtyNodes - qtyLines; i++)
					{
						_curLine._last = _curLine._last._last;
						_curLine._last._next = _curLine;
					}
				}
			}
		}

//		public static xConsole Instance
//		{
//			get
//			{
//				if (console == null)
//					console = new xConsole(20);
//				return console;
//			}
//		}

		public static void Init(int qtyLines)
		{			
			makeNodes(qtyLines);
			_qtyNodes = qtyLines;
		}

		public static void AddLine(string st)
		{
			_curLine = _curLine._last;
			_curLine._st = st;

			if (BufferChanged != null)
				BufferChanged(_curLine);
		}

		public static void SetLine(string st)
		{
			_curLine._st = st;

			if (BufferChanged != null)
				BufferChanged(_curLine);
		}

		public static void AddToLine(string st)
		{
			_curLine._st += st;

			if (BufferChanged != null)
				BufferChanged(_curLine);
		}

//		public void KeyDown(object sender, KeyEventArgs e)
//		{
//			switch(e.KeyCode)
//			{
//				case Keys.Enter:
//					AddLine(command);
//					if (ExecuteCommand != null)
//						ExecuteCommand(command);
//					command = String.Empty;
//					break;
//
//				case Keys.ShiftKey:
//					shift = true;
//					break;
//
//				case Keys.Back:
//					if (command.Length > 0)
//						command = command.Substring(0, command.Length - 1);
//					break;
//
//				case Keys.Oemtilde:
//					if (ExecuteCommand != null)
//						ExecuteCommand("flipLast");
//					break;
//
//				default:
//					char ch = (((char)e.KeyValue) + String.Empty).ToLower()[0];
//
/*
//					if (myFont.KeyValid(ch))
//					{
//						if (shift)
//							command += myFont.ShiftValue(ch);
//						else
//							command += ch;
//					}
//					else
//					{
//						ch = e.KeyCode.ToString()[0];
//						System.Console.WriteLine(e.KeyCode.ToString());
//						if (myFont.KeyValid(ch))
//						{
//							if (shift)
//								command += myFont.ShiftValue(ch);
//							else
//								command += ch;
//						}
//					}
*/
//					break;
//			}
//		}

//		public void KeyUp(object sender, KeyEventArgs e)
//		{
//			switch(e.KeyCode)
//			{
//				case Keys.ShiftKey:
//					shift = false;
//					break;
//			}
//		}
	}


	/// <summary>
	/// class Node
	/// </summary>
	public class Node
	{
		public Node _last;
		public Node _next;
		public string _st;

		/// <summary>
		/// cTor
		/// </summary>
		/// <param name="st"></param>
		public Node(string st)
		{
			_next = null;
			_st = st;
		}
	}
}
