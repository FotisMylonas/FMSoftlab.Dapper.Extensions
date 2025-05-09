using System;
using Xunit;
using Dapper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Data.SqlClient.Server;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace FMSoftlab.Dapper.Extensions.Tests
{
    public class ListItem
    {
        [MaxLength(5)]
        public string Id { get; set; }
    }

    public class DecimalListItem
    {
        [DBAttributeDecimal(18, 4)]
        public decimal Id { get; set; }
    }

    public class VarBinaryMaxItem
    {
        [MaxLength]
        public byte[] Content { get; set; }
    }

    public class DapperCallParameters
    {
        public SqlMapper.ICustomQueryParameter IdList { get; set; }
    }
    public class DapperExtensionsTests
    {


        private static IEnumerable<SqlDataRecord> CreateSqlDataRecords(IEnumerable<ListItem> list)
        {
            var metaData = new SqlMetaData("id", SqlDbType.NVarChar, 5);
            var record = new SqlDataRecord(metaData);
            foreach (var item in list)
            {
                record.SetString(0, item.Id);
                yield return record;
            }
        }
        [Fact]
        public async Task Test_DynamicParameters_GetTableValuedParam()
        {
            string typeName = "dbo.idlist";
            List<ListItem> idlist = new List<ListItem>() { new ListItem { Id = "1" }, new ListItem { Id = "2" } };
            using (SqlConnection con = new SqlConnection(@"Data Source=(LocalDb)\MSSQLLocalDB; Initial Catalog=tempdb; Trusted_Connection=True;"))
            {
                con.Open();
                try
                {
                    await con.ExecuteAsync($"create type {typeName} as table (id nvarchar(5))");
                    DynamicParameters dyn = new DynamicParameters();
                    dyn.Add("idlist", idlist.GetTableValuedParam(typeName));
                    var results = (await con.QueryAsync<string>(@"select id from @idlist idlist", dyn)).ToList();
                    Assert.Equal("1", results[0]);
                    Assert.Equal("2", results[1]);
                }
                finally
                {
                    await con.ExecuteAsync($"drop type {typeName}");
                }
            }
        }

        [Fact]
        public async Task Test_DynamicParameters_AddTableValuedParam()
        {
            string typeName = "dbo.idlist2";
            List<ListItem> idlist = new List<ListItem>() { new ListItem { Id = "1" }, new ListItem { Id = "2" } };
            using (SqlConnection con = new SqlConnection(@"Data Source=(LocalDb)\MSSQLLocalDB; Initial Catalog=tempdb; Trusted_Connection=True;"))
            {
                con.Open();
                try
                {
                    await con.ExecuteAsync($"create type {typeName} as table (id nvarchar(5))");
                    DynamicParameters dyn = new DynamicParameters();
                    dyn.AddTableValuedParam<ListItem>("idlist", typeName, idlist);
                    var results = (await con.QueryAsync<string>(@"select id from @idlist idlist", dyn)).ToList();
                    Assert.Equal("1", results[0]);
                    Assert.Equal("2", results[1]);
                }
                finally
                {
                    await con.ExecuteAsync($"drop type {typeName}");
                }
            }
        }

        [Fact]
        public async Task Test_Anonymous_Object_Param()
        {
            string typeName = "dbo.idlist_Anonymous_Object_Param";
            IEnumerable<ListItem> idlist = new List<ListItem>() { new ListItem { Id = "1" }, new ListItem { Id = "2" } };
            using (SqlConnection con = new SqlConnection(@"Data Source=(LocalDb)\MSSQLLocalDB; Initial Catalog=tempdb; Trusted_Connection=True;"))
            {
                con.Open();
                try
                {
                    await con.ExecuteAsync($"create type {typeName} as table (id nvarchar(5))");
                    var parameters = new
                    {
                        IdList = idlist.GetTableValuedParam<ListItem>(typeName)
                    };
                    var results = (await con.QueryAsync<string>(@"select id from @idlist idlist", parameters)).ToList();
                    Assert.Equal("1", results[0]);
                    Assert.Equal("2", results[1]);
                }
                finally
                {
                    await con.ExecuteAsync($"drop type {typeName}");
                }
            }
        }


        [Fact]
        public async Task Test4_SqlMapper_ICustomQueryParameter_GetTableValuedParam()
        {
            string typeName = "dbo.idlist_Test4";
            IEnumerable<ListItem> idlist = new List<ListItem>() { new ListItem { Id = "1" }, new ListItem { Id = "2" } };
            using (SqlConnection con = new SqlConnection(@"Data Source=(LocalDb)\MSSQLLocalDB; Initial Catalog=tempdb; Trusted_Connection=True;"))
            {
                con.Open();
                try
                {
                    await con.ExecuteAsync($"create type {typeName} as table (id nvarchar(5))");
                    DapperCallParameters parameters = new()
                    {
                        IdList = idlist.GetTableValuedParam<ListItem>(typeName)
                    };
                    var results = con.Query<string>(@"select id from @idlist idlist", parameters).AsList<string>();
                    Assert.Equal("1", results[0]);
                    Assert.Equal("2", results[1]);
                }
                finally
                {
                    await con.ExecuteAsync($"drop type {typeName}");
                }
            }
        }

        [Fact]
        public async Task TestDecimals()
        {
            string typeName = "dbo.idlist_TestDecimals";
            IEnumerable<DecimalListItem> idlist = new List<DecimalListItem>() { new DecimalListItem { Id = 128.4M }, new DecimalListItem { Id = 129.456M } };
            using (SqlConnection con = new SqlConnection(@"Data Source=(LocalDb)\MSSQLLocalDB; Initial Catalog=tempdb; Trusted_Connection=True;"))
            {
                con.Open();
                try
                {
                    await con.ExecuteAsync($"create type {typeName} as table (id decimal(18,4))");
                    DapperCallParameters parameters = new()
                    {
                        IdList = idlist.GetTableValuedParam<DecimalListItem>(typeName)
                    };
                    var results = (await con.QueryAsync<decimal>(@"select id from @idlist idlist", parameters)).ToList();
                    Assert.Equal(128.4M, results[0]);
                    Assert.Equal(129.456M, results[1]);
                }
                finally
                {
                    await con.ExecuteAsync($"drop type {typeName}");
                }
            }
        }

        [Fact]
        public async Task Test_VarBinaryMax()
        {
            string typeName = "dbo.idlist_Test_VarBinaryMax";
            IEnumerable<VarBinaryMaxItem> idlist = new List<VarBinaryMaxItem>() {
                new VarBinaryMaxItem { Content=Encoding.UTF8.GetBytes("Hallo World 1!") },
                new VarBinaryMaxItem { Content=Encoding.UTF8.GetBytes("Hallo World 2!") },
            };
            using (SqlConnection con = new SqlConnection(@"Data Source=(LocalDb)\MSSQLLocalDB; Initial Catalog=tempdb; Trusted_Connection=True;"))
            {
                con.Open();
                try
                {
                    con.Execute($"create type {typeName} as table (content varbinary(max))");
                    DapperCallParameters parameters = new()
                    {
                        IdList = idlist.GetTableValuedParam<VarBinaryMaxItem>(typeName)
                    };
                    var results = (await con.QueryAsync<byte[]>(@"select Content from @idlist idlist", parameters))?.ToList();
                    Assert.Equal("Hallo World 1!", Encoding.UTF8.GetString(results[0]));
                    Assert.Equal("Hallo World 2!", Encoding.UTF8.GetString(results[1]));
                }
                finally
                {
                    con.Execute($"drop type {typeName}");
                }
            }
        }
    }
}