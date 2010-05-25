/// <summary>
/// Represents an collection of Audience Members
/// by Marc Heiligers (marc@madmimi.com)
/// </summary>
using System;
using System.Collections.Generic;
using System.Data;

namespace Vortech.MadMimi
{
	public class AudienceMemberCollection : List<AudienceMember> {
		public ApiResult Import(MailerAPI api) {
			using(DataTable table = ToDataTable()) {
				return api.AudienceImport(table);
			}
		}
		
		public ApiResult Import(MailerAPI api, string listName) {
			using(DataTable table = ToDataTable()) {
				return api.AudienceImport(table, listName);
			}
		}
		
		private DataTable ToDataTable() {
			return ToDataTable(UniqueColumnNames());
		}
		
		private DataTable ToDataTable(List<string> columns) {
			DataTable table = new DataTable("AudienceMembers");
			foreach(string columnName in columns) {
				table.Columns.Add(columnName);
			}
			foreach(AudienceMember member in this) {
				table.Rows.Add(member.ToDataRow(table));
			}
			return table;
		}
		
		private List<string> UniqueColumnNames() {
			List<string> columnNames = new List<string>();
			foreach(AudienceMember member in this) {
				foreach(string columnName in member.ColumnNames()) {
					if(!columnNames.Contains(columnName)) {
						columnNames.Add(columnName);
					}
				}
			}
			return columnNames;
		}
	}

}
