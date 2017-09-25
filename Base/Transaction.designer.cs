﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Base
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="YUANYE")]
	public partial class TransactionDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    partial void InsertS_Transaction(S_Transaction instance);
    partial void UpdateS_Transaction(S_Transaction instance);
    partial void DeleteS_Transaction(S_Transaction instance);
    #endregion
		
		public TransactionDataContext() : 
				base(global::Base.Properties.Settings.Default.YUANYEConnectionString, mappingSource)
		{
			OnCreated();
		}
		
		public TransactionDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public TransactionDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public TransactionDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public TransactionDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<S_Transaction> S_Transactions
		{
			get
			{
				return this.GetTable<S_Transaction>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.S_Transaction")]
	public partial class S_Transaction : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private string _ID;
		
		private string _Code;
		
		private string _Name;
		
		private int _IsCustom;
		
		private string _Action;
		
		private string _Parameters;
		
		private int _NeedRightControl;
		
		private string _RightExpression;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIDChanging(string value);
    partial void OnIDChanged();
    partial void OnCodeChanging(string value);
    partial void OnCodeChanged();
    partial void OnNameChanging(string value);
    partial void OnNameChanged();
    partial void OnIsCustomChanging(int value);
    partial void OnIsCustomChanged();
    partial void OnActionChanging(string value);
    partial void OnActionChanged();
    partial void OnParametersChanging(string value);
    partial void OnParametersChanged();
    partial void OnNeedRightControlChanging(int value);
    partial void OnNeedRightControlChanged();
    partial void OnRightExpressionChanging(string value);
    partial void OnRightExpressionChanged();
    #endregion
		
		public S_Transaction()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ID", DbType="VarChar(50) NOT NULL", CanBeNull=false, IsPrimaryKey=true)]
		public string ID
		{
			get
			{
				return this._ID;
			}
			set
			{
				if ((this._ID != value))
				{
					this.OnIDChanging(value);
					this.SendPropertyChanging();
					this._ID = value;
					this.SendPropertyChanged("ID");
					this.OnIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Code", DbType="VarChar(50) NOT NULL", CanBeNull=false)]
		public string Code
		{
			get
			{
				return this._Code;
			}
			set
			{
				if ((this._Code != value))
				{
					this.OnCodeChanging(value);
					this.SendPropertyChanging();
					this._Code = value;
					this.SendPropertyChanged("Code");
					this.OnCodeChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Name", DbType="VarChar(50) NOT NULL", CanBeNull=false)]
		public string Name
		{
			get
			{
				return this._Name;
			}
			set
			{
				if ((this._Name != value))
				{
					this.OnNameChanging(value);
					this.SendPropertyChanging();
					this._Name = value;
					this.SendPropertyChanged("Name");
					this.OnNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_IsCustom", DbType="Int NOT NULL")]
		public int IsCustom
		{
			get
			{
				return this._IsCustom;
			}
			set
			{
				if ((this._IsCustom != value))
				{
					this.OnIsCustomChanging(value);
					this.SendPropertyChanging();
					this._IsCustom = value;
					this.SendPropertyChanged("IsCustom");
					this.OnIsCustomChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Action", DbType="VarChar(100)")]
		public string Action
		{
			get
			{
				return this._Action;
			}
			set
			{
				if ((this._Action != value))
				{
					this.OnActionChanging(value);
					this.SendPropertyChanging();
					this._Action = value;
					this.SendPropertyChanged("Action");
					this.OnActionChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Parameters", DbType="VarChar(100)")]
		public string Parameters
		{
			get
			{
				return this._Parameters;
			}
			set
			{
				if ((this._Parameters != value))
				{
					this.OnParametersChanging(value);
					this.SendPropertyChanging();
					this._Parameters = value;
					this.SendPropertyChanged("Parameters");
					this.OnParametersChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_NeedRightControl", DbType="Int NOT NULL")]
		public int NeedRightControl
		{
			get
			{
				return this._NeedRightControl;
			}
			set
			{
				if ((this._NeedRightControl != value))
				{
					this.OnNeedRightControlChanging(value);
					this.SendPropertyChanging();
					this._NeedRightControl = value;
					this.SendPropertyChanged("NeedRightControl");
					this.OnNeedRightControlChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_RightExpression", DbType="VarChar(100)")]
		public string RightExpression
		{
			get
			{
				return this._RightExpression;
			}
			set
			{
				if ((this._RightExpression != value))
				{
					this.OnRightExpressionChanging(value);
					this.SendPropertyChanging();
					this._RightExpression = value;
					this.SendPropertyChanged("RightExpression");
					this.OnRightExpressionChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
#pragma warning restore 1591
