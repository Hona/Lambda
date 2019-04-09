using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Dapper.FluentMap;
using Dapper.FluentMap.Configuration;
using MySql.Data.MySqlClient;

namespace LambdaUI.Data
{
    public class MySqlDataAccessBase
    {
        private readonly string _connectionString;
        private MySqlConnection _connection;

        public MySqlDataAccessBase(string connectionString)
        {
            _connectionString = connectionString;
        }
        private async Task OpenConnection()
        {
            if (_connection == null) _connection = new MySqlConnection(_connectionString);
            await _connection.OpenAsync();
        }
        protected async Task CheckConnection()
        {
            if (_connection == null || _connection.State == ConnectionState.Closed || _connection.State == ConnectionState.Broken)
                await OpenConnection();
        }
        internal async Task Close()
        {
            if (_connection != null) await _connection.CloseAsync();
        }

        protected async Task<List<T>> QueryAsync<T>(string query)
        {
            await CheckConnection();
            var result = (await _connection.QueryAsync<T>(query)).ToList();
            Close();
            return result;
        }
        protected async Task<List<T>> QueryAsync<T>(string query, object param)
        {
            await CheckConnection();
            var result = (await _connection.QueryAsync<T>(query, param)).ToList();
            Close();
            return result;
        }
        protected async Task ExecuteAsync(string query, object param)
        {
            await CheckConnection();
            var result = (await _connection.ExecuteAsync(query, param));
            Close();
        }
    }

}
