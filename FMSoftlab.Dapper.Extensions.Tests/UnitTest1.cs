using System;
using Xunit;
using Dapper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Data.SqlClient.Server;
using System.Linq;

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


    public class DapperCallParameters
    {
        public SqlMapper.ICustomQueryParameter IdList { get; set; }
    }
    public class UnitTest1
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
        public void Test1()
        {
            using (SqlConnection con = new SqlConnection(@"Data Source=(LocalDb)\MSSQLLocalDB; Initial Catalog=tempdb; Trusted_Connection=True;"))
            {
                con.Open();
                try
                {
                    con.Execute("create type dbo.idlist as table (id nvarchar(5))");
                    DynamicParameters dyn = new DynamicParameters();
                    List<ListItem> idlist = new List<ListItem>() { new ListItem { Id = "1" }, new ListItem { Id = "2" } };
                    dyn.Add("idlist", idlist.GetTableValuedParam("dbo.idlist"));
                    var results = con.Query<string>(@"select id from @idlist idlist", dyn).ToList<string>();
                    Assert.Equal("1", results[0]);
                    Assert.Equal("2", results[1]);
                }
                finally
                {
                    con.Execute("drop type dbo.idlist");
                }
            }
        }

        [Fact]
        public void Test2()
        {
            using (SqlConnection con = new SqlConnection(@"Data Source=(LocalDb)\MSSQLLocalDB; Initial Catalog=tempdb; Trusted_Connection=True;"))
            {
                con.Open();
                try
                {
                    con.Execute("create type dbo.idlist as table (id nvarchar(5))");
                    DynamicParameters dyn = new DynamicParameters();
                    List<ListItem> idlist = new List<ListItem>() { new ListItem { Id = "1" }, new ListItem { Id = "2" } };
                    dyn.AddTableValuedParam<ListItem>("idlist", "dbo.idlist", idlist);
                    var results = con.Query<string>(@"select id from @idlist idlist", dyn).AsList<string>();
                    Assert.Equal("1", results[0]);
                    Assert.Equal("2", results[1]);
                }
                finally
                {
                    con.Execute("drop type dbo.idlist");
                }
            }
        }

        [Fact]
        public void Test3()
        {
            using (SqlConnection con = new SqlConnection(@"Data Source=(LocalDb)\MSSQLLocalDB; Initial Catalog=tempdb; Trusted_Connection=True;"))
            {
                con.Open();
                try
                {
                    con.Execute("create type dbo.idlist as table (id nvarchar(5))");
                    IEnumerable<ListItem> idlist = new List<ListItem>() { new ListItem { Id = "1" }, new ListItem { Id = "2" } };
                    var parameters = new
                    {
                        IdList = idlist.GetTableValuedParam<ListItem>("dbo.idlist")
                    };
                    var results = con.Query<string>(@"select id from @idlist idlist", parameters).AsList<string>();
                    Assert.Equal("1", results[0]);
                    Assert.Equal("2", results[1]);
                }
                finally
                {
                    con.Execute("drop type dbo.idlist");
                }
            }
        }


        [Fact]
        public void Test4()
        {
            using (SqlConnection con = new SqlConnection(@"Data Source=(LocalDb)\MSSQLLocalDB; Initial Catalog=tempdb; Trusted_Connection=True;"))
            {
                con.Open();
                try
                {
                    con.Execute("create type dbo.idlist as table (id nvarchar(5))");
                    IEnumerable<ListItem> idlist = new List<ListItem>() { new ListItem { Id = "1" }, new ListItem { Id = "2" } };
                    DapperCallParameters parameters = new()
                    {
                        IdList = idlist.GetTableValuedParam<ListItem>("dbo.idlist")
                    };
                    var results = con.Query<string>(@"select id from @idlist idlist", parameters).AsList<string>();
                    Assert.Equal("1", results[0]);
                    Assert.Equal("2", results[1]);
                }
                finally
                {
                    con.Execute("drop type dbo.idlist");
                }
            }
        }

        [Fact]
        public void TestDecimals()
        {
            using (SqlConnection con = new SqlConnection(@"Data Source=(LocalDb)\MSSQLLocalDB; Initial Catalog=tempdb; Trusted_Connection=True;"))
            {
                con.Open();
                try
                {
                    con.Execute("create type dbo.idlist as table (id decimal(18,4))");
                    IEnumerable<DecimalListItem> idlist = new List<DecimalListItem>() { new DecimalListItem { Id = 128.4M }, new DecimalListItem { Id = 129.456M } };
                    DapperCallParameters parameters = new()
                    {
                        IdList = idlist.GetTableValuedParam<DecimalListItem>("dbo.idlist")
                    };
                    var results = con.Query<decimal>(@"select id from @idlist idlist", parameters).AsList<decimal>();
                    Assert.Equal(128.4M, results[0]);
                    Assert.Equal(129.456M, results[1]);
                }
                finally
                {
                    con.Execute("drop type dbo.idlist");
                }
            }
        }
    }
}