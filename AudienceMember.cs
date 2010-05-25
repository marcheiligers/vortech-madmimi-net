/// <summary>
/// Represents an Audience Member
/// by Marc Heiligers (marc@madmimi.com)
/// </summary>

using System;
using System.Collections.Generic;
using System.Data;

namespace Vortech.MadMimi
{
	public class AudienceMember {
		Dictionary<string, string> attributes = new Dictionary<string, string>();
		
		public AudienceMember(string email) {
			if(email == null) {
				throw new ArgumentNullException("email");
			}
			
			attributes.Add("email", email);
		}
		
		public string this[string index] {
			get {
				return attributes.ContainsKey(index) ? attributes[index] : null;
			}
			set {
				if(attributes.ContainsKey(index)) {
					attributes[index] = value;
				} else {
					attributes.Add(index, value);
				}
			}
		}
		
		protected internal List<string> ColumnNames() {
			List<string> columnNames = new List<string>();
			foreach(string columnName in attributes.Keys) {
				columnNames.Add(columnName);
			}
			return columnNames;
		}
		
		protected internal DataRow ToDataRow(DataTable table) {
			DataRow row = table.NewRow();
			foreach(DataColumn column in table.Columns) {
				row[column.ColumnName] = this[column.ColumnName];
			}
			return row;
		}
	}
}
