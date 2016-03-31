using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Base
{
	public class CExpression
	{

		//#################################################
		//名称：CExpression（DataWindow表达式分析类）
		//编程：赵剑英
		//#################################################
		// Option Explicit
		//*************************************************
		//>>常量<<
		//*************************************************
		//-------------------------------------------------
		//调试标志
		//-------------------------------------------------

		//-------------------------------------------------
		//字符串分隔符“"”
		//-------------------------------------------------
		private const string msQUOTATION_MARK  = "\"";

		//-------------------------------------------------
		//表达式分解元素结果
		//-------------------------------------------------
		private enum enuElement
		{
			RADIX_POINT_ONLY = -3,       //仅有小数点
			DOUBLE_RADIX_POINT = -2,     //双重小数点
			EXEPCT_QUOTATION_MARK = -1,  //无法匹配双引号
			NONE = 0,                    //表达式结束
			OPERATOR = 1,                //运算符
			SEPARATOR = 2,               //分隔符
			COMMAND = 3,                 //命令
			DIGITAL = 4,                 //数字
			STRING = 5                  //字符串
		}
		//-------------------------------------------------
		//单目运算符
		//-------------------------------------------------
		private const string msOPERATOR_NEGATIVE  = "-()"; //负号;
		private const string msOPERATOR_NOT = "NOT";      //逻辑非;
		//-------------------------------------------------
		//双目运算符
		//-------------------------------------------------
		//"+", "-", "*", "\", "/", "&",
		//"=","!=",">",">=","<","<="
		private const string msOPERATOR_AND = "AND";      //逻辑与;
		private const string msOPERATOR_OR = "OR" ;       //逻辑或;
		private const string msOPERATOR_MOD = "MOD";      //取模;
		//-------------------------------------------------
		//当前分析状态（for ( InfixToPostFix） {
		//-------------------------------------------------
		private enum enuState
		{
			NEW_EXPRESSION = 0,        //表达式开始
			END_EXPRESSION = 1,        //表达式结束
			SIGLE_OPERATOR = 2,        //单目运算符
			DOUBLE_OPERATOR = 3       //双目运算符
		}
		//*************************************************
		//>>接口<<
		//*************************************************
		//-------------------------------------------------
		//类型：消息
		//功能：请求外部计算自定义函数
		//送出:
		//    Name：函数名称
		//    Arguments：参数堆栈（堆栈从顶到底－参数从前到后）
		//返回：
		//    Value：函数值（空值认为函数错误）
		//-------------------------------------------------
        public delegate void UserFunctionEventHandler(string Name, CStringStack Arguments, ref string Value);
		public event UserFunctionEventHandler UserFunction;
		//-------------------------------------------------
		//类型：消息
		//功能：向外部发送当前堆栈情况
		//送出:
		//    StackType：堆栈类型（1：后缀表达式，2：操作数，3：未完成表达式）
		//    Stack：堆栈（请勿修改）
		//-------------------------------------------------
#if DEBUG 
		public delegate void StackDateEventHandler(int StackType, CStringStack   Stack);
		public event StackDateEventHandler StackDate;
#endif
    
		//-------------------------------------------------
		//功能：计算表达式
		//输入:
		//    sExpression：中缀表达式
		//输出：
		//    sValue：计算结果
		//    sMessage：错误信息
		//    nErrPos：表达式错误位置
		//    Evaluate：（0：成功，-1：分析错误，-2：计算错误）
		//-------------------------------------------------
		public int Evaluate(string sExpression, ref string sValue, ref string sMessage, ref int nErrPos)
		{
			CStringStack clsStack ;
            //string sError;
            //string s;
            //int I;
            //int j;
    
			clsStack = new CStringStack();
			//初步分析
			if ( sExpression == "" ) 
			{
				sValue = "";
				sMessage = "空的表达式。";
				nErrPos = 0;
				clsStack = null;
				return -1;
			}
			//分析表达式
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
			//计算表达式
			if ( ! DoEvaluate(ref clsStack,ref sValue,ref sMessage) ) 
			{
				nErrPos = 0;
				clsStack = null;
				return -2;
			}
			return 0;
		}
		//-------------------------------------------------
		//功能：将中缀表达式转化为后缀表达式
		//输入:
		//    sExpression：中缀表达式
		//输出：
		//    stkPostfix：后缀表达式堆栈
		//    sMessage：错误信息
		//    InfixToPostfix：（0：成功，} else {：表达式中出错位置）
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
							sMessage = "表达式之间需要运算符或分隔符“,”。"; 
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
									sMessage = "“,”只能出现在函数内。"; 
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
										sMessage = "“,”只能出现在函数内。"; 
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
							sMessage = "“,”前必须是完整的表达式。"; 
						} 
					} 
					else if (sElement == ")") 
					{ 
						if (nOldState == enuState.SIGLE_OPERATOR || nOldState == enuState.DOUBLE_OPERATOR) 
						{ 
							sMessage = "运算符后面不能是“)”。"; 
							return  nLastPos;
						} 
						else if (nOldState == enuState.NEW_EXPRESSION || nOldState == enuState.END_EXPRESSION) 
						{ 
							bEndLoop2 = false; 
							do 
							{ 
								if (!(stkOperator.Pop(ref s))) 
								{ 
									sMessage = "无法匹配的右括号。"; 
									return  nLastPos;
								} 
								if (s.StartsWith("(")) 
								{ //是函数
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
										sMessage = "逗号后不能是右括号。"; 
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
							sMessage = "“" + sElement +  "”不是单目运算符。"; 
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
						sMessage = "双重运算符“" + s2 + "”和“"  + sElement + "”。"; 
						return  nLastPos;
					} 
				} 
				else if (element == enuElement.COMMAND) 
				{ 
					//先期取得下一个元素，分析是否是“(”
					I = nLastPos; 
					GetNextElement(sExpression,ref I,ref s); 
					if (s != "(") 
					{ 
						s = ""; 
					} 
					//分析当前命令是否是保留字
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
							sMessage = "“" + s2 + "”和“" + sElement + "”之间缺少运算符。";
							return  nLastPos;
						} 
						else if (nOldState == enuState.SIGLE_OPERATOR || nOldState == enuState.DOUBLE_OPERATOR) 
						{ 
							stkOperator.CheckValue(ref s2); 
							sMessage = "双重运算符“" + s2 + "”和“" + sElement + "”。";
							return  nLastPos;
						} 
					} 
					else if (sElement.ToUpper() == msOPERATOR_AND || sElement.ToUpper() == msOPERATOR_OR || sElement.ToUpper() == msOPERATOR_MOD) 
					{ 
						if (nOldState == enuState.NEW_EXPRESSION) 
						{ 
							sMessage = "运算符“" + sElement + "”前必须是表达式。";
							return  nLastPos;
						} 
						else if (nOldState == enuState.END_EXPRESSION) 
						{ 
							//对所有优先级大于等于自己的运算符压入后缀表达式栈
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
							sMessage = "双重运算符“" + s2 + "”和“" + sElement + "”。";
							return  nLastPos;
						} 
					} 
					else if (sElement.ToUpper() == "TRUE" || sElement.ToUpper() == "FALSE") 
					{ 
						if (s == "(") 
						{ 
							sMessage = "保留字“" + sElement + "”不能作为函数使用。";
							return  nLastPos;
						} 
						else if (nOldState == enuState.END_EXPRESSION) 
						{ 
							sMessage = "“" + sElement + "”之前需要运算符。";
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
							sMessage = "“" + sElement + "”之前需要运算符。";
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
						sMessage = "“" + sElement + "”之前需要运算符。";
					} 
					else 
					{ 
						stkTmp.Push(sElement); 
						nState = enuState.END_EXPRESSION; 
					} 
				} 
				else if (element == enuElement.NONE) 
				{ 
					//处理运算符堆栈
					while (stkOperator.Pop(ref s)) 
					{ 
						if (s.StartsWith("("))
						{ 
							sMessage = "无法匹配的左括号。"; 
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
					sMessage = "无法匹配双引号。"; 
					return  nLastPos;
				} 
				else if (element == enuElement.DOUBLE_RADIX_POINT) 
				{ 
					sMessage = "数字“" + sElement + "”中出现双重的小数点。";
					return  nLastPos;
				} 
				else if (element == enuElement.RADIX_POINT_ONLY) 
				{ 
					sMessage = "仅有小数点的数字。"; 
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
			 CStringStack stkOperator;          //操作层次堆栈;
			 CStringStack stkTmp;              //后缀表达式逆堆栈;
			 int nLastPos;                     //当前已分析位置;
			 enuState nState;
			 enuState nOldState; //当前状态;
			 string sElement;                       //取得表达式元素;
			 bool bEndLoop1;                     //循环条件;
			 bool bEndLoop2;                     //循环条件;
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
								sMessage = "表达式之间需要运算符或分隔符“,”。";
								goto ErrorAttach;
							} 
							else 
							{
								stkOperator.Push sElement;
								nState = enuState.NEW_EXPRESSION;
							}
						case ",":
							//只能出现在函数中并且在表达式之后
							if ( nOldState = enuState.END_EXPRESSION ) 
							{
								bEndLoop2 = False;
								do 
								{ //循环直到匹配（表达式或函数）
									if ( ! stkOperator.CheckValue(ref s) ) 
									{
										sMessage = "“,”只能出现在函数内。";
										goto ErrorAttach;
									}
									switch (s.Substring(0,1))
									{
										case "(":
											if ( s != "(" ) 
											{//是函数
												stkOperator.Push sElement;
												nState = enuState.NEW_EXPRESSION;
												bEndLoop2 = True;
											} 
											else 
											{ //表达式
												sMessage = "“,”只能出现在函数内。";
												goto ErrorAttach;
											}
										case ",": //继承上个“，”的合法性;
											nState = enuState.NEW_EXPRESSION;
											bEndLoop2 = True;
										default: //运算符;
											stkOperator.Pop s2;
											stkTmp.Push s2;
									}
								}  while (bEndLoop2);
							} 
							else 
							{
								sMessage = "“,”前必须是完整的表达式。";
							}
						case ")":
						switch (nOldState) 
						{
							case enuState.SIGLE_OPERATOR:
							case enuState.DOUBLE_OPERATOR:
								sMessage = "运算符后面不能是“)”。";
								goto ErrorAttach;
							case enuState.NEW_EXPRESSION:
							case enuState.END_EXPRESSION:
								bEndLoop2 = False;
								do 
								{ //循环直到匹配（表达式或函数）
									if ( ! stkOperator.Pop(ref s) ) 
									{
										sMessage = "无法匹配的右括号。";
										InfixToPostfix = nLastPos;
										goto ErrorAttach;
									}
									if (s.Substring(0,1) == "(" ) 
									{
										bEndLoop2 = True;
										if ( s != "(" ) 
										{ //是函数
											stkTmp.Push (")" + s.Substring(1));
										}
										else if (s == "," ) 
										{
											if ( nOldState = enuState.NEW_EXPRESSION ) 
											{
												sMessage = "逗号后不能是右括号。";
												goto ErrorAttach;
												//} else { nOldState=enuState.END_EXPRESSION 过滤逗号
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
											//过滤
											nState = enuState.SIGLE_OPERATOR;
											break;
										default:
											sMessage = "“" + sElement + "”不是单目运算符。";
											goto ErrorAttach;
									}
									break;
								case enuState.END_EXPRESSION:
									//对所有优先级大于等于自己的运算符压入后缀表达式栈
									while (stkOperator.CheckValue(ref s))
									{
										if ( GetPrecedence(sElement) > GetPrecedence(s) ) 
										{
											break;
										}
										stkOperator.Pop s2;
										stkTmp.Push s2;
									}
									//加入运算符栈
									stkOperator.Push sElement;
									nState = enuState.DOUBLE_OPERATOR;
									break;
								case enuState.SIGLE_OPERATOR:
								case enuState.DOUBLE_OPERATOR:
									stkOperator.CheckValue s2;
									sMessage = "双重运算符“" + s2 + "”和“" + sElement + "”。";
									goto ErrorAttach;
							}
							break;
						case enuElement.COMMAND:
							//先期取得下一个元素，分析是否是“(”
							I = nLastPos;
							GetNextElement(sExpression, I, s);
							if ( s != "(" ) s = "";
							//分析当前命令是否是保留字
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
										sMessage = "“" + s2 + "”和“" + sElement + "”之间缺少运算符。";
										goto ErrorAttach;
									case enuState.SIGLE_OPERATOR:
									case enuState.DOUBLE_OPERATOR:
										stkOperator.CheckValue s2;
										sMessage = "双重运算符“" + s2 + "”和“" + sElement + "”。";
										goto ErrorAttach;
								}
								case msOPERATOR_AND:
								case msOPERATOR_OR:
								case msOPERATOR_MOD:
								switch (nOldState)
								{
									case enuState.NEW_EXPRESSION:
										sMessage = "运算符“" + sElement + "”前必须是表达式。";
										goto ErrorAttach;
									case enuState.END_EXPRESSION:
										//对所有优先级大于等于自己的运算符压入后缀表达式栈
										while(stkOperator.CheckValue(ref s))
										{
											if ( GetPrecedence(sElement) > GetPrecedence(s) ) 
											{
												break;
											}
											stkOperator.Pop(ref s2);
											stkTmp.Pus(s2);
										}
										//加入运算符栈
										stkOperator.Push(sElement);
										nState = enuState.DOUBLE_OPERATOR;
										break;
									case enuState.SIGLE_OPERATOR:
									case enuState.DOUBLE_OPERATOR:
										stkOperator.CheckValue(ref s2);
										sMessage = "双重运算符“" + s2 + "”和“" + sElement + "”。";
										goto ErrorAttach;
								}
								case "TRUE":
								case "FALSE":
									if ( s = "(" ) 
									{
										sMessage = "保留字“" + sElement + "”不能作为函数使用。";
										goto ErrorAttach;
									}
									else if (nOldState = enuState.END_EXPRESSION ) 
									{
										sMessage = "“" + sElement + "”之前需要运算符。";
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
										sMessage = "“" + sElement + "”之前需要运算符。";
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
								sMessage = "“" + sElement + "”之前需要运算符。";
							} 
							else 
							{
								stkTmp.Push sElement;
								nState = enuState.END_EXPRESSION;
							}
							break;
						case enuElement.NONE:
							//处理运算符堆栈
							while (stkOperator.Pop(ref s))
							{
								if ( s.Substring(0,1) == "(" ) 
								{
									sMessage = "无法匹配的左括号。";
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
							sMessage = "无法匹配双引号。";
							goto ErrorAttach;
						case enuElement.DOUBLE_RADIX_POINT:
							sMessage = "数字“" + sElement + "”中出现双重的小数点。";
							goto ErrorAttach;
						case enuElement.RADIX_POINT_ONLY:
							sMessage = "仅有小数点的数字。";
							goto ErrorAttach;
					}
				}while (bEndLoop1); 
				//从临时堆栈倒入结果堆栈
			while (stkPostfix.Pop(ref s))
			{ 
				//清空结果堆栈
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
		//功能：计算后缀表达式
		//输入:
		//    PostFix：后缀表达式堆栈
		//输出：
		//    sValue：计算结果
		//    DoEvaluate：（True：成功，False：失败）
		//-------------------------------------------------
		public bool DoEvaluate(ref CStringStack PostFix, ref string sValue, ref string sErrMsg) 
		{
			//On Error goto DefaultError;
			CStringStack stkOperand;       //结果堆栈;
			CStringStack stkArg;          //函数参数堆栈;
			string sItem = null;                      //后缀表达式元素;
			string sOP1 = null;
			string sOP2 = null;      //操作数零时变量;
			string sVal = null;                     //计算结果零时变量;
			bool b;                         //零时变量;
			bool bOut = false;
			
			try
			{
				stkOperand = new CStringStack();
				stkArg = new CStringStack();
				while(PostFix.Pop(ref sItem))
				{
					switch (sItem.ToUpper()) 
					{
							//单目运算符
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
									sErrMsg = "单目运算符“-”操作了非数字的值。";
									return false;
								}
							} 
							else 
							{
								sErrMsg = "单目运算符“-”缺少操作数。";
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
								sErrMsg = "单目运算符“" + sItem + "”缺少操作数。";
								return false;
							}
							break;
							//双目运算符
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
									sErrMsg = "双目目运算符“" + sItem + "”操作了非数字的值。";
									return false;
								}
							} 
							else 
							{
								sErrMsg = "双目运算符“" + sItem + "”缺少操作数。";
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
								sErrMsg = "双目运算符“" + sItem + "”缺少操作数。";
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
									case 0://数值比较;
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
											sErrMsg = "比较运算符“" + sItem + "”两边的操作数类型不匹配。";
											return false;
										}
										break;
									case -1: //一边是字符串;
										sErrMsg = "比较运算符“" + sItem + "”两边的操作数类型不匹配。";
										return false;
									case -2 : //两边都是字符串;
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
								sErrMsg = "双目运算符“" + sItem + "”缺少操作数。";
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
									sErrMsg = "双目运算符“" + sItem + "”只能操作逻辑操作数。";
								}
							} 
							else 
							{
								sErrMsg = "双目运算符“" + sItem + "”缺少操作数。";
								return false;
							}
							break;
							//预定义常数
						case "TRUE":
						case "FALSE":
							stkOperand.Push( sItem);
							break;
							//函数或命令
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
								case ".": //数字;
									stkOperand.Push(sItem);
									break;
								case msQUOTATION_MARK: //字符串;
									stkOperand.Push(sItem);
									break;
								case ")" ://函数;
									if ( GetArguments(stkOperand, stkArg) ) 
									{
										switch (sItem.Substring(1).ToUpper()) 
										{
											case "IF":
											case "IIF":
												if (stkArg.StackSize!=3)
												{
													sErrMsg = "函数“" + sItem.Substring(1) + "”的参数树木不匹配。“";
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
													sErrMsg = "函数“" + sItem.Substring(1) + "”的参数树木不匹配。“";
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
													sErrMsg = "函数“" + sItem.Substring(1) + "”的参数树木不匹配。“";
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
													sErrMsg = "函数“" + sItem.Substring(1) + "”的参数树木不匹配。“";
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
													sErrMsg = "错误的自定义函数“" + sItem.Substring(1) + "”。";
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
										sErrMsg = "函数“" + sItem.Substring(1) + "”缺少匹配的括号。";
										return false;
									}
									break;
								default: 
									//命令;
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
					sErrMsg = "操作数与运算符数目不匹配。";
					return false;
				}
			}
			catch
			{
				bOut=false;
				sErrMsg = "操作数“";
				if ( sItem.StartsWith( ")") ) 
				{
					sErrMsg = sErrMsg + sItem.Substring(1);
				} 
				else 
				{
					sErrMsg = sErrMsg + sItem;
				}
				sErrMsg = sErrMsg + "”操作了类型不匹配的操作数。";
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
		//>>函数－－私有<<
		//*************************************************
		//-------------------------------------------------
		//功能：取得下一个表达式元素
		//输入：
		//    sExpression：表达式
		//    nLastPos：表达式中已分析的最后位置
		//返回：
		//    nLastPos：表达式中已分析的最后位置或出错位置
		//    sTmp：取得的元素
		//    GetNextElement：元素类型
		//运行过程:
		//    取得第一个有意义字符，分析元素类型
		//    1、结束
		//        stmp = ""
		//        GetNextElement = enuElement.NONE
		//    2、运算符
		//        stmp = <运算符>
		//        GetNextElement = enuElement.OPERATOR
		//    3、分隔符
		//        stmp = <分隔符>
		//        GetNextElement = enuElement.SEPARATOR
		//    4、字符串
		//        4.1、
		//            stmp = <以双引号包括的字符串>
		//            GetNextElement = enuElement.STRING
		//        4.2、没有匹配的右双引号
		//            stmp = <只有左双引号包括的字符串>
		//            GetNextElement = enuElement.EXEPCT_QUOTATION_MARK
		//    5、数字
		//        5.1、
		//            stmp = <数字>
		//            GetNextElement = enuElement.DIGITAL
		//        5.2、双重小数点
		//            stmp = <已取得的数字>
		//            GetNextElement = enuElement.DOUBLE_RADIX_POINT
		//        5.3、仅有小数点
		//            stmp = <小数点>
		//            GetNextElement = enuElement.RADIX_POINT_ONLY
		//    6 、其他
		//        stmp = <命令>
		//        GetNextElement = enuElement.COMMAND
		//-------------------------------------------------
		private enuElement GetNextElement(string sExpression, ref int nLastPos, ref string sElement) 
		{
			enuElement eOut = enuElement.NONE;
			string ch1 = "";
			string ch2 ="";
			//string ch3 =""; //临时字符变量;
			string sTmp;                             //取得元素的临时变量;
			bool bEndLoop;                       //循环条件;
			bool bRadixPoint;                     //数字型元素的小数点计数;
    
			sTmp = "";
            do
            { //取得第一个有意义字符
                nLastPos = nLastPos + 1;
                if (sExpression.Length <= nLastPos)
                {
                    ch1 = "";
                    break;
                }
                ch1 = sExpression.Substring(nLastPos, 1);
            } while (ch1.Length > 0 && ch1[0] <= " "[0]);  //(!( ch1.CompareTo(" ")>0 || ch1 == ""));
			switch (ch1) 
			{  //分析不同类型;
					//结束
				case "":
					sTmp = "";
					eOut = enuElement.NONE;
					break;
					//分隔符
				case "(":
				case ")":
				case ",":
					sTmp = ch1;
					eOut = enuElement.SEPARATOR;
					break;
					//运算符
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
					{ //结束或其他
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
					{ //结束或其他
						sTmp = ">";
					}
					break;
					//字符串
				case msQUOTATION_MARK:
					sTmp = msQUOTATION_MARK;
					bEndLoop = false;
					do 
					{
						nLastPos = nLastPos + 1;
						ch2 = sExpression.Substring(nLastPos, 1);
						switch (ch2) 
						{
							case "": //结束;
								sElement = sTmp;
								return enuElement.EXEPCT_QUOTATION_MARK;
							case msQUOTATION_MARK:
								sTmp = sTmp + msQUOTATION_MARK;
								if ( sExpression.Substring(nLastPos+1, 1) == msQUOTATION_MARK ) 
								{
									//连续双引号为转义符，表示字符串中的单个双引号
									nLastPos = nLastPos + 1;
								} 
								else 
								{
									//单个双引号，表示字符串结束
									eOut = enuElement.STRING;
									bEndLoop = true;
								}
								break;
							default:
								//字符串内容;
								sTmp = sTmp + ch2;
								break;
						}
						nLastPos = nLastPos + 1;
					}while (!bEndLoop); 
					break;
					//数字
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
							default://数字结束;
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
				default://命令;
					sTmp = ch1;
					bEndLoop = false;
					do 
					{
						ch2 = sExpression.Substring(nLastPos+1, 1);
						switch (ch2) 
						{
							case "": //结束;
								bEndLoop = true;
								break;
							case "(":
							case ")":
							case ",": //分隔符;
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
							case ">": //运算符;
								bEndLoop = true;
								break;
							case msQUOTATION_MARK:
								bEndLoop = true;
								break;
							case " ":
							case "\t"://多余符;
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
		//功能：取得优先权
		//输入:
		//    Operator：运算符
		//输出：
		//    GetPrecedence：优先权级别
		//说明：优先权级别值大的优先权高
		//-------------------------------------------------
		private int GetPrecedence(string Operator) 
		{
			switch (Operator.ToUpper())
			{
					//算术运算符
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
					//比较运算符
				case "=":
				case "!=":
				case ">":
				case ">=": 
				case "<":
				case "<=":
					return  19;
					//逻辑运算符
				case msOPERATOR_NOT:
					return  9;
				case msOPERATOR_AND:
					return  8;
				case msOPERATOR_OR:
					return  7;
					//非运算符
				default:
					return  0;
			}
		}
		//-------------------------------------------------
		//功能：取得函数参数
		//输入:
		//    Operand：操作数堆栈
		//输出：
		//    Arguments：参数堆栈
		//    GetArguments：（True：成功，False：失败）
		//调用者： Evaluate() {
		//-------------------------------------------------
		private bool GetArguments(CStringStack Operand ,CStringStack Arguments) 
		{
			string s = null;
    
			while(Arguments.Pop(ref s)) 
			{ 
				//清空参数堆栈
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