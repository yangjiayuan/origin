using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Base
{
	public class CExpression
	{

		//#################################################
		//���ƣ�CExpression��DataWindow���ʽ�����ࣩ
		//��̣��Խ�Ӣ
		//#################################################
		// Option Explicit
		//*************************************************
		//>>����<<
		//*************************************************
		//-------------------------------------------------
		//���Ա�־
		//-------------------------------------------------

		//-------------------------------------------------
		//�ַ����ָ�����"��
		//-------------------------------------------------
		private const string msQUOTATION_MARK  = "\"";

		//-------------------------------------------------
		//���ʽ�ֽ�Ԫ�ؽ��
		//-------------------------------------------------
		private enum enuElement
		{
			RADIX_POINT_ONLY = -3,       //����С����
			DOUBLE_RADIX_POINT = -2,     //˫��С����
			EXEPCT_QUOTATION_MARK = -1,  //�޷�ƥ��˫����
			NONE = 0,                    //���ʽ����
			OPERATOR = 1,                //�����
			SEPARATOR = 2,               //�ָ���
			COMMAND = 3,                 //����
			DIGITAL = 4,                 //����
			STRING = 5                  //�ַ���
		}
		//-------------------------------------------------
		//��Ŀ�����
		//-------------------------------------------------
		private const string msOPERATOR_NEGATIVE  = "-()"; //����;
		private const string msOPERATOR_NOT = "NOT";      //�߼���;
		//-------------------------------------------------
		//˫Ŀ�����
		//-------------------------------------------------
		//"+", "-", "*", "\", "/", "&",
		//"=","!=",">",">=","<","<="
		private const string msOPERATOR_AND = "AND";      //�߼���;
		private const string msOPERATOR_OR = "OR" ;       //�߼���;
		private const string msOPERATOR_MOD = "MOD";      //ȡģ;
		//-------------------------------------------------
		//��ǰ����״̬��for ( InfixToPostFix�� {
		//-------------------------------------------------
		private enum enuState
		{
			NEW_EXPRESSION = 0,        //���ʽ��ʼ
			END_EXPRESSION = 1,        //���ʽ����
			SIGLE_OPERATOR = 2,        //��Ŀ�����
			DOUBLE_OPERATOR = 3       //˫Ŀ�����
		}
		//*************************************************
		//>>�ӿ�<<
		//*************************************************
		//-------------------------------------------------
		//���ͣ���Ϣ
		//���ܣ������ⲿ�����Զ��庯��
		//�ͳ�:
		//    Name����������
		//    Arguments��������ջ����ջ�Ӷ����ף�������ǰ����
		//���أ�
		//    Value������ֵ����ֵ��Ϊ��������
		//-------------------------------------------------
        public delegate void UserFunctionEventHandler(string Name, CStringStack Arguments, ref string Value);
		public event UserFunctionEventHandler UserFunction;
		//-------------------------------------------------
		//���ͣ���Ϣ
		//���ܣ����ⲿ���͵�ǰ��ջ���
		//�ͳ�:
		//    StackType����ջ���ͣ�1����׺���ʽ��2����������3��δ��ɱ��ʽ��
		//    Stack����ջ�������޸ģ�
		//-------------------------------------------------
#if DEBUG 
		public delegate void StackDateEventHandler(int StackType, CStringStack   Stack);
		public event StackDateEventHandler StackDate;
#endif
    
		//-------------------------------------------------
		//���ܣ�������ʽ
		//����:
		//    sExpression����׺���ʽ
		//�����
		//    sValue��������
		//    sMessage��������Ϣ
		//    nErrPos�����ʽ����λ��
		//    Evaluate����0���ɹ���-1����������-2���������
		//-------------------------------------------------
		public int Evaluate(string sExpression, ref string sValue, ref string sMessage, ref int nErrPos)
		{
			CStringStack clsStack ;
            //string sError;
            //string s;
            //int I;
            //int j;
    
			clsStack = new CStringStack();
			//��������
			if ( sExpression == "" ) 
			{
				sValue = "";
				sMessage = "�յı��ʽ��";
				nErrPos = 0;
				clsStack = null;
				return -1;
			}
			//�������ʽ
			nErrPos = InfixToPostfix(sExpression,ref clsStack,ref sMessage);
			if ( nErrPos != 0 ) 
			{
				sValue = "";
				clsStack = null;
				return -1;
			}
#if DEBUG
			if (StackDate!=null)
				StackDate(1, clsStack);
#endif
			//������ʽ
			if ( ! DoEvaluate(ref clsStack,ref sValue,ref sMessage) ) 
			{
				nErrPos = 0;
				clsStack = null;
				return -2;
			}
			return 0;
		}
		//-------------------------------------------------
		//���ܣ�����׺���ʽת��Ϊ��׺���ʽ
		//����:
		//    sExpression����׺���ʽ
		//�����
		//    stkPostfix����׺���ʽ��ջ
		//    sMessage��������Ϣ
		//    InfixToPostfix����0���ɹ���} else {�����ʽ�г���λ�ã�
		//-------------------------------------------------
		public int InfixToPostfix(string sExpression, ref CStringStack stkPostfix, ref string sMessage) 
		{ 
			CStringStack stkOperator; 
			CStringStack stkTmp; 
			int nLastPos; 
			enuState nState; 
			enuState nOldState; 
			string sElement = null; 
			bool bEndLoop1; 
			bool bEndLoop2; 
			string s = null; 
			string s2 =null; 
			int I; 
			stkOperator = new CStringStack(); 
			stkTmp = new CStringStack(); 
			nState =  enuState.NEW_EXPRESSION; 
			nLastPos = -1; 
			bEndLoop1 = false; 
			do 
			{ 
				nOldState = nState; 
				enuElement element = GetNextElement(sExpression,ref nLastPos,ref sElement);
				if (element == enuElement.SEPARATOR) 
				{ 
					if (sElement == "(") 
					{ 
						if (nOldState == enuState.END_EXPRESSION) 
						{ 
							sMessage = "���ʽ֮����Ҫ�������ָ�����,����"; 
							return  nLastPos;
						} 
						else 
						{ 
							stkOperator.Push(sElement); 
							nState = enuState.NEW_EXPRESSION; 
						} 
					} 
					else if (sElement == ",") 
					{ 
						if (nOldState == enuState.END_EXPRESSION) 
						{ 
							bEndLoop2 = false; 
							do 
							{ 
								if (!(stkOperator.CheckValue(ref s))) 
								{ 
									sMessage = "��,��ֻ�ܳ����ں����ڡ�"; 
									return  nLastPos;
								} 
								if (s.StartsWith("(")) 
								{ 
									if (s != "(") 
									{ 
										stkOperator.Push(sElement); 
										nState = enuState.NEW_EXPRESSION; 
										bEndLoop2 = true; 
									} 
									else 
									{ 
										sMessage = "��,��ֻ�ܳ����ں����ڡ�"; 
										return  nLastPos;
									} 
								} 
								else if (s.StartsWith(","))
								{ 
									nState = enuState.NEW_EXPRESSION; 
									bEndLoop2 = true; 
								} 
								else 
								{ 
									stkOperator.Pop(ref s2); 
									stkTmp.Push(s2); 
								} 
							} while (!(bEndLoop2)); 
						} 
						else 
						{ 
							sMessage = "��,��ǰ�����������ı��ʽ��"; 
						} 
					} 
					else if (sElement == ")") 
					{ 
						if (nOldState == enuState.SIGLE_OPERATOR || nOldState == enuState.DOUBLE_OPERATOR) 
						{ 
							sMessage = "��������治���ǡ�)����"; 
							return  nLastPos;
						} 
						else if (nOldState == enuState.NEW_EXPRESSION || nOldState == enuState.END_EXPRESSION) 
						{ 
							bEndLoop2 = false; 
							do 
							{ 
								if (!(stkOperator.Pop(ref s))) 
								{ 
									sMessage = "�޷�ƥ��������š�"; 
									return  nLastPos;
								} 
								if (s.StartsWith("(")) 
								{ //�Ǻ���
									bEndLoop2 = true; 
									if (s != "(") 
									{ 
										stkTmp.Push(")" + s.Substring(1)); 
									} 
								} 
								else if (s == ",") 
								{ 
									if (nOldState == enuState.NEW_EXPRESSION) 
									{ 
										sMessage = "���ź����������š�"; 
										return  nLastPos;
									} 
								} 
								else 
								{ 
									stkTmp.Push(s); 
								} 
								nState = enuState.END_EXPRESSION; 
							} while (!(bEndLoop2)); 
						} 
					} 
				} 
				else if (element == enuElement.OPERATOR) 
				{ 
					if (nOldState == enuState.NEW_EXPRESSION) 
					{ 
						if (sElement == "-") 
						{ 
							stkOperator.Push(msOPERATOR_NEGATIVE); 
							nState = enuState.SIGLE_OPERATOR; 
						} 
						else if (sElement == "+") 
						{ 
							nState = enuState.SIGLE_OPERATOR; 
						} 
						else 
						{ 
							sMessage = "��" + sElement +  "�����ǵ�Ŀ�������"; 
							return  nLastPos;
						} 
					} 
					else if (nOldState == enuState.END_EXPRESSION) 
					{ 
						while (stkOperator.CheckValue(ref s)) 
						{ 
							if (GetPrecedence(sElement) > GetPrecedence(s)) 
							{ 
								break;
							} 
							stkOperator.Pop(ref s2); 
							stkTmp.Push(s2); 
						} 
						stkOperator.Push(sElement); 
						nState = enuState.DOUBLE_OPERATOR; 
					} 
					else if (nOldState == enuState.SIGLE_OPERATOR || nOldState == enuState.DOUBLE_OPERATOR) 
					{ 
						stkOperator.CheckValue(ref s2); 
						sMessage = "˫���������" + s2 + "���͡�"  + sElement + "����"; 
						return  nLastPos;
					} 
				} 
				else if (element == enuElement.COMMAND) 
				{ 
					//����ȡ����һ��Ԫ�أ������Ƿ��ǡ�(��
					I = nLastPos; 
					GetNextElement(sExpression,ref I,ref s); 
					if (s != "(") 
					{ 
						s = ""; 
					} 
					//������ǰ�����Ƿ��Ǳ�����
					if (sElement.ToUpper() == msOPERATOR_NOT) 
					{ 
						if (nOldState == enuState.NEW_EXPRESSION) 
						{ 
							stkOperator.Push(sElement); 
							nState = enuState.SIGLE_OPERATOR; 
						} 
						else if (nOldState == enuState.END_EXPRESSION) 
						{ 
							stkTmp.CheckValue(ref s2); 
							sMessage = "��" + s2 + "���͡�" + sElement + "��֮��ȱ���������";
							return  nLastPos;
						} 
						else if (nOldState == enuState.SIGLE_OPERATOR || nOldState == enuState.DOUBLE_OPERATOR) 
						{ 
							stkOperator.CheckValue(ref s2); 
							sMessage = "˫���������" + s2 + "���͡�" + sElement + "����";
							return  nLastPos;
						} 
					} 
					else if (sElement.ToUpper() == msOPERATOR_AND || sElement.ToUpper() == msOPERATOR_OR || sElement.ToUpper() == msOPERATOR_MOD) 
					{ 
						if (nOldState == enuState.NEW_EXPRESSION) 
						{ 
							sMessage = "�������" + sElement + "��ǰ�����Ǳ��ʽ��";
							return  nLastPos;
						} 
						else if (nOldState == enuState.END_EXPRESSION) 
						{ 
							//���������ȼ����ڵ����Լ��������ѹ���׺���ʽջ
							while (stkOperator.CheckValue(ref s)) 
							{ 
								if (GetPrecedence(sElement) > GetPrecedence(s)) 
								{ 
									break; 
								} 
								stkOperator.Pop(ref s2); 
								stkTmp.Push(s2); 
							} 
							stkOperator.Push(sElement); 
							nState = enuState.DOUBLE_OPERATOR; 
						} 
						else if (nOldState == enuState.SIGLE_OPERATOR || nOldState == enuState.DOUBLE_OPERATOR) 
						{ 
							stkOperator.CheckValue(ref s2); 
							sMessage = "˫���������" + s2 + "���͡�" + sElement + "����";
							return  nLastPos;
						} 
					} 
					else if (sElement.ToUpper() == "TRUE" || sElement.ToUpper() == "FALSE") 
					{ 
						if (s == "(") 
						{ 
							sMessage = "�����֡�" + sElement + "��������Ϊ����ʹ�á�";
							return  nLastPos;
						} 
						else if (nOldState == enuState.END_EXPRESSION) 
						{ 
							sMessage = "��" + sElement + "��֮ǰ��Ҫ�������";
						} 
						else 
						{ 
							stkTmp.Push(sElement); 
							nState = enuState.END_EXPRESSION; 
						} 
					} 
					else 
					{ 
						if (nOldState == enuState.END_EXPRESSION) 
						{ 
							sMessage = "��" + sElement + "��֮ǰ��Ҫ�������";
						} 
						if (s == "(") 
						{ 
							stkTmp.Push("("); 
							stkOperator.Push("(" + sElement); 
							nState = enuState.NEW_EXPRESSION; 
							nLastPos = I; 
						} 
						else 
						{ 
							stkTmp.Push(sElement); 
							nState = enuState.END_EXPRESSION; 
						} 
					} 
				} 
				else if (element == enuElement.DIGITAL || element == enuElement.STRING) 
				{ 
					if (nOldState == enuState.END_EXPRESSION) 
					{ 
						sMessage = "��" + sElement + "��֮ǰ��Ҫ�������";
					} 
					else 
					{ 
						stkTmp.Push(sElement); 
						nState = enuState.END_EXPRESSION; 
					} 
				} 
				else if (element == enuElement.NONE) 
				{ 
					//�����������ջ
					while (stkOperator.Pop(ref s)) 
					{ 
						if (s.StartsWith("("))
						{ 
							sMessage = "�޷�ƥ��������š�"; 
							return  nLastPos;
						} 
						else 
						{ 
							stkTmp.Push(s); 
						} 
					} 
					bEndLoop1 = true; 
				} 
				else if (element == enuElement.EXEPCT_QUOTATION_MARK) 
				{ 
					sMessage = "�޷�ƥ��˫���š�"; 
					return  nLastPos;
				} 
				else if (element == enuElement.DOUBLE_RADIX_POINT) 
				{ 
					sMessage = "���֡�" + sElement + "���г���˫�ص�С���㡣";
					return  nLastPos;
				} 
				else if (element == enuElement.RADIX_POINT_ONLY) 
				{ 
					sMessage = "����С��������֡�"; 
					return  nLastPos;
				} 
			} while (!(bEndLoop1)); 

			while (stkPostfix.Pop(ref s)) 
			{ 
			} 
//			CStringStack ss = new CStringStack();
//			while (stkTmp.Pop(ref s)) 
//			{ 
//				ss.Push(s); 
//			}
			while (stkTmp.Pop(ref s)) 
			{ 
				stkPostfix.Push(s); 
			} 
			//InfixToPostfix = 0; 
			return 0; 

		}
		/*public int InfixToPostfix(string sExpression,CStringStack stkPostfix, ref string sMessage ) 
		{
			 CStringStack stkOperator;          //������ζ�ջ;
			 CStringStack stkTmp;              //��׺���ʽ���ջ;
			 int nLastPos;                     //��ǰ�ѷ���λ��;
			 enuState nState;
			 enuState nOldState; //��ǰ״̬;
			 string sElement;                       //ȡ�ñ��ʽԪ��;
			 bool bEndLoop1;                     //ѭ������;
			 bool bEndLoop2;                     //ѭ������;
			 string s;
			 string s2;
			 int I;
    
			stkOperator = new CStringStack();
			stkTmp = new CStringStack();
			nState = enuState.NEW_EXPRESSION;
			nLastPos = 0;
			bEndLoop1 = False;
			do 
			{
				nOldState = nState;
				switch (GetNextElement(sExpression,ref nLastPos,ref sElement))
				{
					case enuElement.SEPARATOR:
					switch (sElement) 
					{ 
						case "(":
							if ( nOldState = enuState.END_EXPRESSION ) 
							{
								sMessage = "���ʽ֮����Ҫ�������ָ�����,����";
								goto ErrorAttach;
							} 
							else 
							{
								stkOperator.Push sElement;
								nState = enuState.NEW_EXPRESSION;
							}
						case ",":
							//ֻ�ܳ����ں����в����ڱ��ʽ֮��
							if ( nOldState = enuState.END_EXPRESSION ) 
							{
								bEndLoop2 = False;
								do 
								{ //ѭ��ֱ��ƥ�䣨���ʽ������
									if ( ! stkOperator.CheckValue(ref s) ) 
									{
										sMessage = "��,��ֻ�ܳ����ں����ڡ�";
										goto ErrorAttach;
									}
									switch (s.Substring(0,1))
									{
										case "(":
											if ( s != "(" ) 
											{//�Ǻ���
												stkOperator.Push sElement;
												nState = enuState.NEW_EXPRESSION;
												bEndLoop2 = True;
											} 
											else 
											{ //���ʽ
												sMessage = "��,��ֻ�ܳ����ں����ڡ�";
												goto ErrorAttach;
											}
										case ",": //�̳��ϸ��������ĺϷ���;
											nState = enuState.NEW_EXPRESSION;
											bEndLoop2 = True;
										default: //�����;
											stkOperator.Pop s2;
											stkTmp.Push s2;
									}
								}  while (bEndLoop2);
							} 
							else 
							{
								sMessage = "��,��ǰ�����������ı��ʽ��";
							}
						case ")":
						switch (nOldState) 
						{
							case enuState.SIGLE_OPERATOR:
							case enuState.DOUBLE_OPERATOR:
								sMessage = "��������治���ǡ�)����";
								goto ErrorAttach;
							case enuState.NEW_EXPRESSION:
							case enuState.END_EXPRESSION:
								bEndLoop2 = False;
								do 
								{ //ѭ��ֱ��ƥ�䣨���ʽ������
									if ( ! stkOperator.Pop(ref s) ) 
									{
										sMessage = "�޷�ƥ��������š�";
										InfixToPostfix = nLastPos;
										goto ErrorAttach;
									}
									if (s.Substring(0,1) == "(" ) 
									{
										bEndLoop2 = True;
										if ( s != "(" ) 
										{ //�Ǻ���
											stkTmp.Push (")" + s.Substring(1));
										}
										else if (s == "," ) 
										{
											if ( nOldState = enuState.NEW_EXPRESSION ) 
											{
												sMessage = "���ź����������š�";
												goto ErrorAttach;
												//} else { nOldState=enuState.END_EXPRESSION ���˶���
											}
										} 
										else 
										{
											stkTmp.Push( s);
										}
										nState = enuState.END_EXPRESSION;
									}
								}while (bEndLoop2);
						}
							break;
						case enuElement.OPERATOR:
							switch (nOldState) 
							{
								case enuState.NEW_EXPRESSION:
									switch (sElement) 
									{
										case "-":
											stkOperator.Push msOPERATOR_NEGATIVE;
											nState = enuState.SIGLE_OPERATOR;
											break;
										case "+":
											//����
											nState = enuState.SIGLE_OPERATOR;
											break;
										default:
											sMessage = "��" + sElement + "�����ǵ�Ŀ�������";
											goto ErrorAttach;
									}
									break;
								case enuState.END_EXPRESSION:
									//���������ȼ����ڵ����Լ��������ѹ���׺���ʽջ
									while (stkOperator.CheckValue(ref s))
									{
										if ( GetPrecedence(sElement) > GetPrecedence(s) ) 
										{
											break;
										}
										stkOperator.Pop s2;
										stkTmp.Push s2;
									}
									//���������ջ
									stkOperator.Push sElement;
									nState = enuState.DOUBLE_OPERATOR;
									break;
								case enuState.SIGLE_OPERATOR:
								case enuState.DOUBLE_OPERATOR:
									stkOperator.CheckValue s2;
									sMessage = "˫���������" + s2 + "���͡�" + sElement + "����";
									goto ErrorAttach;
							}
							break;
						case enuElement.COMMAND:
							//����ȡ����һ��Ԫ�أ������Ƿ��ǡ�(��
							I = nLastPos;
							GetNextElement(sExpression, I, s);
							if ( s != "(" ) s = "";
							//������ǰ�����Ƿ��Ǳ�����
							switch (sElement.ToUpper())
							{
								case msOPERATOR_NOT:
								switch (nOldState) 
								{
									case enuState.NEW_EXPRESSION:
										stkOperator.Push sElement;
										nState = enuState.SIGLE_OPERATOR;
										break;
									case enuState.END_EXPRESSION:
										stkTmp.CheckValue s2;
										sMessage = "��" + s2 + "���͡�" + sElement + "��֮��ȱ���������";
										goto ErrorAttach;
									case enuState.SIGLE_OPERATOR:
									case enuState.DOUBLE_OPERATOR:
										stkOperator.CheckValue s2;
										sMessage = "˫���������" + s2 + "���͡�" + sElement + "����";
										goto ErrorAttach;
								}
								case msOPERATOR_AND:
								case msOPERATOR_OR:
								case msOPERATOR_MOD:
								switch (nOldState)
								{
									case enuState.NEW_EXPRESSION:
										sMessage = "�������" + sElement + "��ǰ�����Ǳ��ʽ��";
										goto ErrorAttach;
									case enuState.END_EXPRESSION:
										//���������ȼ����ڵ����Լ��������ѹ���׺���ʽջ
										while(stkOperator.CheckValue(ref s))
										{
											if ( GetPrecedence(sElement) > GetPrecedence(s) ) 
											{
												break;
											}
											stkOperator.Pop(ref s2);
											stkTmp.Pus(s2);
										}
										//���������ջ
										stkOperator.Push(sElement);
										nState = enuState.DOUBLE_OPERATOR;
										break;
									case enuState.SIGLE_OPERATOR:
									case enuState.DOUBLE_OPERATOR:
										stkOperator.CheckValue(ref s2);
										sMessage = "˫���������" + s2 + "���͡�" + sElement + "����";
										goto ErrorAttach;
								}
								case "TRUE":
								case "FALSE":
									if ( s = "(" ) 
									{
										sMessage = "�����֡�" + sElement + "��������Ϊ����ʹ�á�";
										goto ErrorAttach;
									}
									else if (nOldState = enuState.END_EXPRESSION ) 
									{
										sMessage = "��" + sElement + "��֮ǰ��Ҫ�������";
									} 
									else 
									{
										stkTmp.Push sElement;
										nState = enuState.END_EXPRESSION;
									}
									break;
								default:
									if ( nOldState == enuState.END_EXPRESSION )
									{
										sMessage = "��" + sElement + "��֮ǰ��Ҫ�������";
									}
									if ( s == "(" )
									{
										stkTmp.Push("(");
										stkOperator.Push("(" + sElement);
										nState = enuState.NEW_EXPRESSION;
										nLastPos = I;
									} 
									else 
									{
										stkTmp.Push sElement;
										nState = enuState.END_EXPRESSION;
									}
									break;
							}
						case enuElement.DIGITAL:
						case enuElement.STRING:
							if ( nOldState = enuState.END_EXPRESSION ) 
							{
								sMessage = "��" + sElement + "��֮ǰ��Ҫ�������";
							} 
							else 
							{
								stkTmp.Push sElement;
								nState = enuState.END_EXPRESSION;
							}
							break;
						case enuElement.NONE:
							//�����������ջ
							while (stkOperator.Pop(ref s))
							{
								if ( s.Substring(0,1) == "(" ) 
								{
									sMessage = "�޷�ƥ��������š�";
									goto ErrorAttach;
								} 
								else 
								{
									stkTmp.Push( s);
								}
							}
							bEndLoop1 = True;
							break;
						case enuElement.EXEPCT_QUOTATION_MARK:
							sMessage = "�޷�ƥ��˫���š�";
							goto ErrorAttach;
						case enuElement.DOUBLE_RADIX_POINT:
							sMessage = "���֡�" + sElement + "���г���˫�ص�С���㡣";
							goto ErrorAttach;
						case enuElement.RADIX_POINT_ONLY:
							sMessage = "����С��������֡�";
							goto ErrorAttach;
					}
				}while (bEndLoop1); 
				//����ʱ��ջ��������ջ
			while (stkPostfix.Pop(ref s))
			{ 
				//��ս����ջ
			}
			while (stkTmp.Pop(ref s))
			{ 
				stkPostfix.Push s;
			}
				return 0;
			ErrorAttach:
				return  nLastPos;
			}*/
		private bool IsTextValidated(string strTextEntry)
		{
            double v = 0;
            try
            {
                v = double.Parse(strTextEntry);
                return true;
            }
            catch
            {
                return false;
            }
		}
		//-------------------------------------------------
		//���ܣ������׺���ʽ
		//����:
		//    PostFix����׺���ʽ��ջ
		//�����
		//    sValue��������
		//    DoEvaluate����True���ɹ���False��ʧ�ܣ�
		//-------------------------------------------------
		public bool DoEvaluate(ref CStringStack PostFix, ref string sValue, ref string sErrMsg) 
		{
			//On Error goto DefaultError;
			CStringStack stkOperand;       //�����ջ;
			CStringStack stkArg;          //����������ջ;
			string sItem = null;                      //��׺���ʽԪ��;
			string sOP1 = null;
			string sOP2 = null;      //��������ʱ����;
			string sVal = null;                     //��������ʱ����;
			bool b;                         //��ʱ����;
			bool bOut = false;
			
			try
			{
				stkOperand = new CStringStack();
				stkArg = new CStringStack();
				while(PostFix.Pop(ref sItem))
				{
					switch (sItem.ToUpper()) 
					{
							//��Ŀ�����
						case "-()":
							if ( stkOperand.Pop(ref sOP1) ) 
							{
								if ( sOP1.StartsWith(msQUOTATION_MARK) ) 
								{ sOP1 = sOP1.Substring(1, sOP1.Length - 2);}
								if ( IsTextValidated(sOP1) ) 
								{
									sVal =( -1 * Double.Parse(sOP1)).ToString();
									stkOperand.Push(sVal);
								} 
								else 
								{
									sErrMsg = "��Ŀ�������-�������˷����ֵ�ֵ��";
									return false;
								}
							} 
							else 
							{
								sErrMsg = "��Ŀ�������-��ȱ�ٲ�������";
								return false;
							}
							break;
						case "NOT":
							if ( stkOperand.Pop(ref sOP1) ) 
							{
								if ( sOP1.StartsWith(msQUOTATION_MARK) ) { sOP1 =sOP1.Substring(1, sOP1.Length- 2);}
								sVal = (! bool.Parse(sOP1)).ToString();
								stkOperand.Push(sVal.ToUpper());
							} 
							else 
							{
								sErrMsg = "��Ŀ�������" + sItem + "��ȱ�ٲ�������";
								return false;
							}
							break;
							//˫Ŀ�����
						case "+":
						case "-":
						case "*": 
						case "/":
						case "\\":
						case "^": 
						case "MOD":
							b = stkOperand.Pop(ref sOP1);
							b = b && stkOperand.Pop(ref sOP2);
							if ( b ) 
							{
								if ( IsTextValidated(sOP1) && IsTextValidated(sOP2) ) 
								{
									double v = 0;
									switch (sItem.ToUpper()) 
									{
										case "+":
											v = double.Parse(sOP2) + double.Parse(sOP1);break;
										case "-":
											v = double.Parse(sOP2) - double.Parse(sOP1);break;
										case "*":
											v = double.Parse(sOP2) * double.Parse(sOP1);break;
										case "/":
											v = double.Parse(sOP2) / double.Parse(sOP1);break;
										case "\\":
											v = int.Parse(sOP2) / int.Parse(sOP1);break;
										case "^":
											v = Math.Pow(double.Parse(sOP2) , double.Parse(sOP1));break;
										case "MOD":
											v =  int.Parse(sOP2) % int.Parse(sOP1);
											break;
									}
									sVal = v.ToString();
									stkOperand.Push(sVal);
								} 
								else 
								{
									sErrMsg = "˫ĿĿ�������" + sItem + "�������˷����ֵ�ֵ��";
									return false;
								}
							} 
							else 
							{
								sErrMsg = "˫Ŀ�������" + sItem + "��ȱ�ٲ�������";
								return false;
							}
							break;
						case "&":
							b = stkOperand.Pop(ref sOP1);
							b = b && stkOperand.Pop(ref sOP2);
							if ( b ) 
							{
								if ( sOP1.StartsWith(msQUOTATION_MARK) ) { sOP1 = sOP1.Substring(1, sOP1.Length - 2);}
								if ( sOP2.StartsWith(msQUOTATION_MARK)) { sOP2 = sOP2.Substring(2, sOP2.Length - 2);}
								sVal = msQUOTATION_MARK + sOP2 + sOP1 + msQUOTATION_MARK;
								stkOperand.Push(sVal);
							} 
							else 
							{
								sErrMsg = "˫Ŀ�������" + sItem + "��ȱ�ٲ�������";
								return false;
							}
							break;
						case "=":
						case "!=":
						case ">":
						case ">=":
						case "<":
						case "<=":
							b = stkOperand.Pop(ref sOP1);
							b = b && stkOperand.Pop(ref sOP2);
							if ( b ) 
							{
								switch ( (sOP1.StartsWith(msQUOTATION_MARK)?-1:0) + (sOP2.StartsWith(msQUOTATION_MARK)?-1:0) )
								{
									case 0://��ֵ�Ƚ�;
										if ( IsTextValidated(sOP1) && IsTextValidated(sOP2) ) 
										{
											switch (sItem) 
											{
												case "=":
													sVal = (double.Parse(sOP2) == double.Parse(sOP1)).ToString();break;
												case "!=":
													sVal = (double.Parse(sOP2) != double.Parse(sOP1)).ToString();break;
												case ">":
													sVal = (double.Parse(sOP2) > double.Parse(sOP1)).ToString();break;
												case ">=":
													sVal = (double.Parse(sOP2) >= double.Parse(sOP1)).ToString();break;
												case "<":
													sVal = (double.Parse(sOP2) < double.Parse(sOP1)).ToString();break;
												case "<=":
													sVal = (double.Parse(sOP2) <= double.Parse(sOP1)).ToString();break;
											}
											stkOperand.Push(sVal);
										} 
										else 
										{
											sErrMsg = "�Ƚ��������" + sItem + "�����ߵĲ��������Ͳ�ƥ�䡣";
											return false;
										}
										break;
									case -1: //һ�����ַ���;
										sErrMsg = "�Ƚ��������" + sItem + "�����ߵĲ��������Ͳ�ƥ�䡣";
										return false;
									case -2 : //���߶����ַ���;
										sOP1 = sOP1.Substring(1, sOP1.Length - 2);
										sOP2 = sOP2.Substring(1, sOP2.Length - 2);
										switch (sItem) 
										{
											case "=":
												sVal = (sOP2 == sOP1).ToString();break;
											case "!=":
												sVal = (string.Equals(sOP2,sOP1)).ToString();break;
											case ">":
												sVal = (string.Compare(sOP2 , sOP1)> sOP1.Length).ToString() ;break;
											case ">=":
												sVal = (string.Compare( sOP2 , sOP1)>=sOP1.Length).ToString();break;
											case "<":
												sVal = (string.Compare(sOP1,sOP2 )>sOP2.Length).ToString();break;
											case "<=":
												sVal = (string.Compare(sOP1,sOP2)>=sOP2.Length).ToString();break;
										}
										stkOperand.Push(sVal);
										break;
							}
							} 
							else 
							{
								sErrMsg = "˫Ŀ�������" + sItem + "��ȱ�ٲ�������";
								return false;
							}
							break;
						case "AND":
						case "OR":
							b = stkOperand.Pop(ref sOP1);
							b = b && stkOperand.Pop(ref sOP2);
							if ( b ) 
							{
								sOP1 = sOP1.ToUpper();
								sOP2 = sOP2.ToUpper();
								if ( ((sOP1 == "TRUE") || (sOP1 == "FALSE")) && ((sOP2 == "TRUE") || (sOP2 == "FALSE")) ) 
								{
									switch (sItem.ToUpper()) 
									{
										case "AND":
											sVal = (bool.Parse(sOP2) && bool.Parse(sOP1)).ToString();break;
										case "OR":
											sVal = (bool.Parse(sOP2) || bool.Parse(sOP1)).ToString();break;
									}
									stkOperand.Push(sVal);
								} 
								else 
								{
									sErrMsg = "˫Ŀ�������" + sItem + "��ֻ�ܲ����߼���������";
								}
							} 
							else 
							{
								sErrMsg = "˫Ŀ�������" + sItem + "��ȱ�ٲ�������";
								return false;
							}
							break;
							//Ԥ���峣��
						case "TRUE":
						case "FALSE":
							stkOperand.Push( sItem);
							break;
							//����������
						default:
							switch (sItem.Substring(0,1)) 
							{
								case "0":
								case "1":
								case "2":
								case "3":
								case "4":
								case "5":
								case "6":
								case "7":
								case "8":
								case "9":
								case ".": //����;
									stkOperand.Push(sItem);
									break;
								case msQUOTATION_MARK: //�ַ���;
									stkOperand.Push(sItem);
									break;
								case ")" ://����;
									if ( GetArguments(stkOperand, stkArg) ) 
									{
										switch (sItem.Substring(1).ToUpper()) 
										{
											case "IF":
											case "IIF":
												if (stkArg.StackSize!=3)
												{
													sErrMsg = "������" + sItem.Substring(1) + "���Ĳ�����ľ��ƥ�䡣��";
												}
												else
												{
													stkArg.Pop(ref sOP1);
													bool bv = bool.Parse(sOP1);
													stkArg.Pop(ref sOP1);
													stkArg.Pop(ref sOP2);
													if (bv)
														stkOperand.Push(sOP1);
													else
														stkOperand.Push(sOP2);
												}
												break;
											case "CBOOL":
												if ( stkArg.StackSize != 1 ) 
												{
													sErrMsg = "������" + sItem.Substring(1) + "���Ĳ�����ľ��ƥ�䡣��";
												} 
												else 
												{
													stkArg.Pop(ref sOP1);
													if ( sOP1.StartsWith(msQUOTATION_MARK) ) 
													{
														sOP1 = sOP1.Substring(1, sOP1.Length - 2);
														sVal = (bool.Parse(sOP1)).ToString();
														stkOperand.Push(sVal);
													}
												}
												break;
											case "CSTR":
												if ( stkArg.StackSize != 1 ) 
												{
													sErrMsg = "������" + sItem.Substring(1) + "���Ĳ�����ľ��ƥ�䡣��";
												} 
												else 
												{
													stkArg.Pop(ref sOP1);
													if ( sOP1.StartsWith(msQUOTATION_MARK) ) 
													{
														sVal = sOP1;
													} 
													else 
													{
														sVal = msQUOTATION_MARK + sOP1 + msQUOTATION_MARK;
													}
													stkOperand.Push(sVal);
												}
												break;
											case "CDEC":
												if ( stkArg.StackSize != 1 ) 
												{
													sErrMsg = "������" + sItem.Substring(1) + "���Ĳ�����ľ��ƥ�䡣��";
												} 
												else 
												{
													stkArg.Pop(ref sOP1);
													if ( sOP1.StartsWith(msQUOTATION_MARK) ) 
														sOP1 = sOP1.Substring(1, sOP1.Length - 2);
													if ( sOP1.ToUpper() == "TRUE" ) 
													{
														sVal = "-1";
													}
													else if (sOP1.ToUpper() == "FASLE" ) 
													{
														sVal = "0";
													} 
													else 
													{
														sVal = (decimal.Parse(sOP1)).ToString();
													}
													stkOperand.Push(sVal);
												}
												break;
											default:
												if (UserFunction!=null)
													UserFunction(sItem.Substring(1), stkArg, ref sVal);
												if ( sVal == "" ) 
												{
													sErrMsg = "������Զ��庯����" + sItem.Substring(1) + "����";
													return false;
												} 
												else 
												{
													stkOperand.Push(sVal);
												}
												break;
										}
									}
									else
									{
										sErrMsg = "������" + sItem.Substring(1) + "��ȱ��ƥ������š�";
										return false;
									}
									break;
								default: 
									//����;
									stkOperand.Push(sItem);
									break;
							}
							break;
					}
	
				}
				if ( stkOperand.StackSize == 1 ) 
				{
					stkOperand.Pop(ref sValue);
					sErrMsg = "";
					return true;
				} 
				else 
				{
					sErrMsg = "���������������Ŀ��ƥ�䡣";
					return false;
				}
			}
			catch
			{
				bOut=false;
				sErrMsg = "��������";
				if ( sItem.StartsWith( ")") ) 
				{
					sErrMsg = sErrMsg + sItem.Substring(1);
				} 
				else 
				{
					sErrMsg = sErrMsg + sItem;
				}
				sErrMsg = sErrMsg + "�����������Ͳ�ƥ��Ĳ�������";
			}
			finally
			{
				stkArg = null;
				stkOperand = null;
			}
			return bOut;
#if DEBUG
			//        RaiseEvent StackDate(2, stkOperand);
			//        RaiseEvent StackDate(3, PostFix);
#endif

		}
		//*************************************************
		//>>��������˽��<<
		//*************************************************
		//-------------------------------------------------
		//���ܣ�ȡ����һ�����ʽԪ��
		//���룺
		//    sExpression�����ʽ
		//    nLastPos�����ʽ���ѷ��������λ��
		//���أ�
		//    nLastPos�����ʽ���ѷ��������λ�û����λ��
		//    sTmp��ȡ�õ�Ԫ��
		//    GetNextElement��Ԫ������
		//���й���:
		//    ȡ�õ�һ���������ַ�������Ԫ������
		//    1������
		//        stmp = ""
		//        GetNextElement = enuElement.NONE
		//    2�������
		//        stmp = <�����>
		//        GetNextElement = enuElement.OPERATOR
		//    3���ָ���
		//        stmp = <�ָ���>
		//        GetNextElement = enuElement.SEPARATOR
		//    4���ַ���
		//        4.1��
		//            stmp = <��˫���Ű������ַ���>
		//            GetNextElement = enuElement.STRING
		//        4.2��û��ƥ�����˫����
		//            stmp = <ֻ����˫���Ű������ַ���>
		//            GetNextElement = enuElement.EXEPCT_QUOTATION_MARK
		//    5������
		//        5.1��
		//            stmp = <����>
		//            GetNextElement = enuElement.DIGITAL
		//        5.2��˫��С����
		//            stmp = <��ȡ�õ�����>
		//            GetNextElement = enuElement.DOUBLE_RADIX_POINT
		//        5.3������С����
		//            stmp = <С����>
		//            GetNextElement = enuElement.RADIX_POINT_ONLY
		//    6 ������
		//        stmp = <����>
		//        GetNextElement = enuElement.COMMAND
		//-------------------------------------------------
		private enuElement GetNextElement(string sExpression, ref int nLastPos, ref string sElement) 
		{
			enuElement eOut = enuElement.NONE;
			string ch1 = "";
			string ch2 ="";
			//string ch3 =""; //��ʱ�ַ�����;
			string sTmp;                             //ȡ��Ԫ�ص���ʱ����;
			bool bEndLoop;                       //ѭ������;
			bool bRadixPoint;                     //������Ԫ�ص�С�������;
    
			sTmp = "";
            do
            { //ȡ�õ�һ���������ַ�
                nLastPos = nLastPos + 1;
                if (sExpression.Length <= nLastPos)
                {
                    ch1 = "";
                    break;
                }
                ch1 = sExpression.Substring(nLastPos, 1);
            } while (ch1.Length > 0 && ch1[0] <= " "[0]);  //(!( ch1.CompareTo(" ")>0 || ch1 == ""));
			switch (ch1) 
			{  //������ͬ����;
					//����
				case "":
					sTmp = "";
					eOut = enuElement.NONE;
					break;
					//�ָ���
				case "(":
				case ")":
				case ",":
					sTmp = ch1;
					eOut = enuElement.SEPARATOR;
					break;
					//�����
				case "+":
				case "-": 
				case "*":
				case "\\":
				case "/":
				case "&":
				case "=":
				case "^":
					sTmp = ch1;
					eOut = enuElement.OPERATOR;
					break;
				case "<":
					eOut = enuElement.OPERATOR;
					ch2 = sExpression.Substring(nLastPos+1, 1);
					if ( (ch2 == "=") || (ch2 == ">") ) 
					{
						sTmp = ch1 + ch2;
						nLastPos = nLastPos + 1;
					} 
					else 
					{ //����������
						sTmp = "<";
					}
					break;
				case ">":
					eOut = enuElement.OPERATOR;
					ch2 = sExpression.Substring(nLastPos+1, 1);
					if ( (ch2 == "=") ) 
					{
						sTmp = ">=";
						nLastPos = nLastPos + 1;
					} 
					else 
					{ //����������
						sTmp = ">";
					}
					break;
					//�ַ���
				case msQUOTATION_MARK:
					sTmp = msQUOTATION_MARK;
					bEndLoop = false;
					do 
					{
						nLastPos = nLastPos + 1;
						ch2 = sExpression.Substring(nLastPos, 1);
						switch (ch2) 
						{
							case "": //����;
								sElement = sTmp;
								return enuElement.EXEPCT_QUOTATION_MARK;
							case msQUOTATION_MARK:
								sTmp = sTmp + msQUOTATION_MARK;
								if ( sExpression.Substring(nLastPos+1, 1) == msQUOTATION_MARK ) 
								{
									//����˫����Ϊת�������ʾ�ַ����еĵ���˫����
									nLastPos = nLastPos + 1;
								} 
								else 
								{
									//����˫���ţ���ʾ�ַ�������
									eOut = enuElement.STRING;
									bEndLoop = true;
								}
								break;
							default:
								//�ַ�������;
								sTmp = sTmp + ch2;
								break;
						}
						nLastPos = nLastPos + 1;
					}while (!bEndLoop); 
					break;
					//����
				case "0":
				case "1":
				case "2":
				case "3":
				case "4":
				case "5":
				case "6":
				case "7":
				case "8":
				case "9":
				case ".":
					sTmp = ch1;
					if ( ch1 == "." ) 
					{
						bRadixPoint = true;
					} 
					else 
					{
						bRadixPoint = false;
					}
					bEndLoop = false;
					do 
					{
						if (sExpression.Length<= nLastPos+1)
							break;
						ch2 = sExpression.Substring(nLastPos+1, 1);
						switch (ch2) 
						{
							case "0":
							case "1":
							case "2":
							case "3":
							case "4":
							case "5":
							case "6":
							case "7":
							case "8":
							case "9":
								sTmp = sTmp + ch2;
								nLastPos = nLastPos + 1;
								break;
							case ".":
								sTmp = sTmp + ch2;
								if ( bRadixPoint ) 
								{
									sElement = sTmp;
									return enuElement.DOUBLE_RADIX_POINT;
								} 
								else 
								{
									bRadixPoint = true;
								}
								nLastPos = nLastPos + 1;
								break;
							default://���ֽ���;
								bEndLoop = true;
								break;
						}
					}  while (!bEndLoop); 
					if ( sTmp == "." ) 
					{
						sElement = sTmp;
						return enuElement.RADIX_POINT_ONLY;
					} 
					else 
					{
						eOut = enuElement.DIGITAL;
					}
					break;
				default://����;
					sTmp = ch1;
					bEndLoop = false;
					do 
					{
						ch2 = sExpression.Substring(nLastPos+1, 1);
						switch (ch2) 
						{
							case "": //����;
								bEndLoop = true;
								break;
							case "(":
							case ")":
							case ",": //�ָ���;
								bEndLoop = true;
								break;
							case "+":
							case "-":
							case "*":
							case "\\":
							case "/": 
							case "&":
							case "=":
							case "<":
							case ">": //�����;
								bEndLoop = true;
								break;
							case msQUOTATION_MARK:
								bEndLoop = true;
								break;
							case " ":
							case "\t"://�����;
								bEndLoop = true;
								break;
							default:
								sTmp = sTmp + ch2;
								nLastPos = nLastPos + 1;
								break;
						}
					}  while (!bEndLoop); 
					eOut = enuElement.COMMAND;
					break;
			}
			sElement = sTmp;
			return eOut;
		}
		//-------------------------------------------------
		//���ܣ�ȡ������Ȩ
		//����:
		//    Operator�������
		//�����
		//    GetPrecedence������Ȩ����
		//˵��������Ȩ����ֵ�������Ȩ��
		//-------------------------------------------------
		private int GetPrecedence(string Operator) 
		{
			switch (Operator.ToUpper())
			{
					//���������
				case "^":
					return  29;
				case msOPERATOR_NEGATIVE:
					return  28;
				case "*":
				case "/":
					return  27;
				case "\\":
					return  26;
				case msOPERATOR_MOD:
					return  25;
				case "+":
				case "-":
					return  24;
				case "&":
					return  23;
					//�Ƚ������
				case "=":
				case "!=":
				case ">":
				case ">=": 
				case "<":
				case "<=":
					return  19;
					//�߼������
				case msOPERATOR_NOT:
					return  9;
				case msOPERATOR_AND:
					return  8;
				case msOPERATOR_OR:
					return  7;
					//�������
				default:
					return  0;
			}
		}
		//-------------------------------------------------
		//���ܣ�ȡ�ú�������
		//����:
		//    Operand����������ջ
		//�����
		//    Arguments��������ջ
		//    GetArguments����True���ɹ���False��ʧ�ܣ�
		//�����ߣ� Evaluate() {
		//-------------------------------------------------
		private bool GetArguments(CStringStack Operand ,CStringStack Arguments) 
		{
			string s = null;
    
			while(Arguments.Pop(ref s)) 
			{ 
				//��ղ�����ջ
			}
			bool rOut = false;
			while(Operand.Pop(ref s))
			{
				if ( s == "(" ) 
				{
					 rOut = true;
					break;
				} 
				else 
				{
					Arguments.Push(s);
				}
			}
			return rOut;
		}
	}
}