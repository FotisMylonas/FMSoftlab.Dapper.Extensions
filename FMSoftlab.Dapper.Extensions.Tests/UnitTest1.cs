using System;
using Xunit;
using Dapper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace FMSoftlab.Dapper.Extensions.Tests
{
    public class ListItem
    {
        [MaxLength(5)]
        public string Id { get; set; }
    }

    public class DapperCallParameters
    {
        public SqlMapper.ICustomQueryParameter IdList { get; set; }
    }
    public class UnitTest1
    {
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
                    IEnumerable<ListItem> idlist = new List<ListItem>() { new ListItem { Id = "1" }, new ListItem { Id = "2" } };
                    dyn.Add("idlist", idlist.GetTableValuedParam<ListItem>("dbo.idlist"));
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
        public void Test2()
        {
            using (SqlConnection con = new SqlConnection(@"Data Source=(LocalDb)\MSSQLLocalDB; Initial Catalog=tempdb; Trusted_Connection=True;"))
            {
                con.Open();
                try
                {
                    con.Execute("create type dbo.idlist as table (id nvarchar(5))");
                    DynamicParameters dyn = new DynamicParameters();
                    IEnumerable<ListItem> idlist = new List<ListItem>() { new ListItem { Id = "1" }, new ListItem { Id = "2" } };
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
    }
}