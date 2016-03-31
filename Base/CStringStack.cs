using System;
using System.Collections;

namespace Base
{
	public class CStringStack
	{
		//#################################################
		//名称：CStringStack（字符串堆栈）
		//编程：赵剑英
		//#################################################
		//>>变量——私有<<
		//*************************************************
		private ArrayList  mavStack = new ArrayList();        //堆栈体;

		//*************************************************
		//>>属性<<
	    //*************************************************
		//-------------------------------------------------
		//堆栈库存数
		//-------------------------------------------------
		public int StackSize  
		{
			get{return mavStack.Count;}
		}
		//*************************************************
		//>>方法<<
		//*************************************************
		//-------------------------------------------------
		//功能：压栈
		//输入：
		//    Value：压栈数据
		//说明：自动调节堆栈容量
		//-------------------------------------------------
		public void Push(string Value) 
		{
			mavStack.Add(Value);
		}
		//-------------------------------------------------
		//功能：出栈
		//输出：
		//    Value：出栈数据
		//    Pop：（True：成功，False：堆栈已空）
		//-------------------------------------------------
		public bool Pop(ref  string Value) 
		{
			if ( mavStack.Count > 0 ) 
			{
				Value = (string)mavStack[mavStack.Count-1];
				mavStack.RemoveAt(mavStack.Count-1);
				return true;
			}
			else 
			{
				Value = "";
				return false;
			}
		}
		//-------------------------------------------------
		//功能：检查指定数据
		//输出：
		//    Value：数据
		//    CheckValue：（True：成功，False：堆栈已空）
		//-------------------------------------------------
		public bool CheckValue(ref string Value) 
		{
			return CheckValue(ref Value,mavStack.Count-1);
		}
		public bool CheckValue(ref string Value,int StackPos)
		{
			if (mavStack.Count>0)
			{
				if (StackPos>=mavStack.Count || StackPos<0)
					StackPos = mavStack.Count-1;
				Value = (string)mavStack[StackPos];
				return true;
			}
			else
			{
				Value="";
				return false;
			}
		}
	}
}