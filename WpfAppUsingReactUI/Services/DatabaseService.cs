using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using restTest.Models;


/// <summary>
/// Relational database is a set of tables (i.e customers, invoices, products, employees, suppliers)
/// Table = class , columns = attributes, rows = objects
/// Normalization check whether attributes are in the correct table preventing inconsistent data (different aspects of the info need to be separated into normalized tables)
/// Data model displays foreign key constraints
/// SQL keywords should be written in capitals / field values, table names should be written in lowercase
/// Foreign key already exist as primary key of other table
/// AS name the column of the resulted output table
/// Using Aliases using tableName in from of variableName when variables of different tables have same name
/// Use alias to avoid typing the whole table name
/// A subquery can be considered as a variable
/// 
///        SELECT (ALL/*/DISTINCT/TOP) column_list AS resul_list 
///        FROM table_list or subquery 
///        (INNER/LEFT/RIGHT/FULL) JOIN table ON join_condition(tb1.key = tb2.key)
///        WHERE filter_condition
///              expr (equal, bigger than, smaller than) value 
///              expr AND/OR expr
///              column IS NULL
///              column [NOT] IN(value1, ..., valueN / subquery)
///              column [NOT] BETWEEN(value1, value2)
///              column LIKE patern_%
///              [NOT] EXIST(subquery) 
///        GROUP BY column or expression
///        ORDER BY column(ASC/DESC) 
///        HAVING row_grouping_condition
///        LIMIT count OFFSET offset 
///        UNION 
/// 
/// SELECT allow to specify a subset of columns
/// SELECT DISTINT _field outputs result without duplicates
/// SELECT column_list FROM (subquery). In this case, query only can use the resulted columns of the subquery
/// ORDER BY can use columnName or columnNumber (c1 = 1, c2=2, ...)
/// OFFSET means how many rows to be jumped
/// WHERE returns a subset of rows that satisfy a given condition (boolean expression) and it is applied to each row
/// WHERE is a part of the SELECT query as a whole, ON is a part of each individual join
/// IN can be used as a subquery inside WHERE
/// WHERE describes which rows we are interested in
/// JOIN allows to combine rows from two or more tables, based on a related column between them
/// SELF JOIN is used where there is any relationship between rows sotred in the same table (which employee is boss of each employee)
/// Like use OR
/// GROUP BY applied to groups or sum up items with sharing attributes. ie. all tracks of the same album
/// HAVING filter after aggregation
/// COUNT, MAX, MIN, SUM, AVG are aggregate functions
/// Aggregate functions ignore null rows from computation
/// Count(X) function returns a count of the number of times that X is not NULL in a group
/// Sum(X)  returns the sum of all non-NULL values in the group
/// EXIST is a logical operator that specifies a subquery to test
/// Views can be used to save a query for next time 
/// @ can be used to form a verbatim string literal
/// </summary>

namespace WpfAppUsingReactUI.Services
{
    public class DatabaseService
    {
        List<Artist> _lookup;
        SQLiteAsyncConnection con;
        public DatabaseService()
        {
            var databasePath = @"D:\Databases\chinook.db";
            con = new SQLiteAsyncConnection(databasePath);
        }

        public async Task<Tuple<string, List<Artist>>> FetchArtists(string searchTerm)
        {
            string query = "SELECT DISTINCT * FROM artists WHERE Name LIKE ? ORDER BY Name ASC";
            await Task.Delay(1000);// pretend the search take long
            var t = await con.QueryAsync<Artist>(query, searchTerm + "%");
            return new Tuple<string, List<Artist>>(searchTerm, t);
        }

        public Task<Tuple<string, List<Artist>>> FetchResults(string searchTerm)
        {
            return Task<List<string>>.Run(async () =>
            {
                await Task.Delay(1000);// pretend the search take long
                if (_lookup == null)
                {
                    _lookup = _lookup ?? await LoadAsync();
                }
                var ret = _lookup.Where(x => x.Name.Contains(searchTerm)).ToList();//filter method
                return new Tuple<string, List<Artist>>(searchTerm, ret);
            });
        }
        public async Task<List<Artist>> LoadAsync()
        {
            string query = "SELECT * FROM artists";
            await Task.Delay(1000);// pretend the search take long
            var t = await con.QueryAsync<Artist>(query);

            return t;
        }

        public class stringField
        {
            public string text { set; get; }
        }

        public class intField
        {
            public int value { set; get; }
        }


        /*
        private static void Main(string[] args)
        {
            // Get an absolute path to the database file
            var databasePath = Path.Combine(Environment.CurrentDirectory, "chinook.db");
            SQLiteConnection conn = new SQLiteConnection(databasePath);
            string query = "SELECT * FROM artists";

            //sqlite_master is the system table in SQLite
            query = "SELECT name FROM sqlite_master WHERE type IN ('table','view') AND name NOT LIKE 'sqlite_%' ORDER BY 1";
            var tableNames = conn.Query<stringField>(query);
            query = "SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name NOT LIKE 'sqlite_%' ORDER BY 1";
            var tablesNum = conn.ExecuteScalar<int>(query); //11

            query = "SELECT InvoiceId FROM invoices where CustomerId = 4";
            var res = conn.Query<intField>(query);//7 results (2,24,76,197,208,263,392)
            query = "SELECT InvoiceId FROM invoices WHERE CustomerId = 4 AND InvoiceDate BETWEEN '2009-01-11' AND '2011-08-11'";
            var res2 = conn.Query<intField>(query);//4 results (24,76,197,208)
            query = "SELECT SUM(InvoiceId) FROM invoices WHERE CustomerId = 4 AND InvoiceDate BETWEEN '2009-01-11' AND '2011-08-11'";
            var count = conn.ExecuteScalar<int>(query); //505

            //Start with E or W or end with k ordered by lenght
            query = "SELECT CustomerId,FirstName FROM customers WHERE FirstName Like 'e%' OR FirstName Like 'w%' OR FirstName Like '%k' ORDER BY length(FirstName) ASC";
            var res3 = conn.Query<stringField>(query);//13 results 


            query = "SELECT ar.Name as ArtistName, al.Title as AlbumName FROM artists ar JOIN albums al ON ar.ArtistId = al.ArtistId WHERE ar.ArtistId =8";


            //search albums of a artist called like C something and ordered alphabetically
            query = @"SELECT ar.Name AS ArtistName,
                             al.Title AS AlbumName
                     FROM artists ar
                             JOIN
                             albums al ON ar.ArtistId = al.ArtistId
                     WHERE ar.Name LIKE 'c%'
                     ORDER BY ar.Name ASC";

            //number of tracks for each album using GROUP BY
            query = "SELECT AlbumId, Name COUNT(trackid) FROM tracks GROUP BY AlbumId ORDER BY AlbumId ASC";

            //Get entire ar and alb of first purchase of customer 4 
            query = @"SELECT ar.Name as ArtistName, al.Title as AlbumName FROM artists ar, invoices inv JOIN invoice_items it ON it.invoiceId = inv.InvoiceId JOIN albums al ON al.albumid = tk.albumid AND ar.ArtistId = al.ArtistId JOIN tracks tk ON tk.TrackId = it.TrackId WHERE  inv.CustomerId = 4 ORDER BY Total Asc limit 1";
            var res4 = conn.Query<stringField>(query);

            //Top 3 customers based on total 
            query = "SELECT c.CustomerId as ID, c.FirstName, c.LastName, i.Total FROM customers c JOIN invoices i ON c.CustomerId = i.CustomerId ORDER BY i.Total DESC LIMIT 3";

            //shortest and longest track based on duration (seconds) using UNION
            query = @"SELECT Name,
                             MIN(Milliseconds / 1000) AS Duration
                        FROM tracks
                      UNION
                      SELECT Name,
                             MAX(Milliseconds / 1000) AS Duration
                        FROM tracks";

            //Set new categories based on track lenght using CASE
            query = @"SELECT trackid, name,
                     CASE
                     WHEN milliseconds < 60000 THEN 'short'
                     WHEN milliseconds > 6000 AND milliseconds < 300000 THEN 'medium'
                     ELSE 'long'
                     END category
                    FROM tracks";
            var res6 = conn.Query<stringField>(query);

            //best 5 selling artists based on amound of selled tracks (iron maiden, u2, metallica, led zeppeling, os paralamas do sucesso)
            query = @"SELECT ArtistName,
                             SUM(orders) AS sells
                        FROM (
                               SELECT ii.TrackId,
                                      t.Name AS TrackName,
                                      ar.Name AS ArtistName,
                                      COUNT(ii.invoiceId) AS orders
                                 FROM invoice_items ii
                                      JOIN
                                      tracks t ON ii.trackId = t.trackId
                                      JOIN
                                      albums al ON al.albumid = t.albumid
                                      JOIN
                                      artists ar ON ar.ArtistId = al.ArtistId
                                GROUP BY ii.TrackId
                                ORDER BY orders DESC
                           )
                       GROUP BY ArtistName
                       ORDER BY sells DESC
                       LIMIT 5";

        }
        */

    }
}
