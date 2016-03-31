using System;
using System.Collections;

namespace Base
{
	public class CStringStack
	{
		//#################################################
		//���ƣ�CStringStack���ַ�����ջ��
		//��̣��Խ�Ӣ
		//#################################################
		//>>��������˽��<<
		//*************************************************
		private ArrayList  mavStack = new ArrayList();        //��ջ��;

		//*************************************************
		//>>����<<
	    //*************************************************
		//-------------------------------------------------
		//��ջ�����
		//-------------------------------------------------
		public int StackSize  
		{
			get{return mavStack.Count;}
		}
		//*************************************************
		//>>����<<
		//*************************************************
		//-------------------------------------------------
		//���ܣ�ѹջ
		//���룺
		//    Value��ѹջ����
		//˵�����Զ����ڶ�ջ����
		//-------------------------------------------------
		public void Push(string Value) 
		{
			mavStack.Add(Value);
		}
		//-------------------------------------------------
		//���ܣ���ջ
		//�����
		//    Value����ջ����
		//    Pop����True���ɹ���False����ջ�ѿգ�
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
		//���ܣ����ָ������
		//�����
		//    Value������
		//    CheckValue����True���ɹ���False����ջ�ѿգ�
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