using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using LambdaUI.Logging;
using MySql.Data.MySqlClient;

namespace LambdaUI.Data.Access
{
    public class MySqlDataAccessBase
    {
        private readonly string _connectionString;
        private MySqlConnection _connection;

        protected MySqlDataAccessBase(string connectionString) => _connectionString = connectionString;

        private async Task OpenConnectionAsync()
        {
            if (_connection == null) _connection = new MySqlConnection(_connectionString);
            await _connection.OpenAsync();
        }

        private async Task CheckConnectionAsync()
        {
            if (_connection == null || _connection.State == ConnectionState.Closed ||
                _connection.State == ConnectionState.Broken)
                await OpenConnectionAsync();
        }

        private async void CloseAsync()
        {
            if (_connection != null) await _connection.CloseAsync();
        }

        protected async Task<List<T>> QueryAsync<T>(string query)
        {
            try
            {
                await CheckConnectionAsync();
                var result = (await _connection.QueryAsync<T>(query)).ToList();
                CloseAsync();
                return result;
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return null;
            }

        }

        protected async Task<List<T>> QueryAsync<T>(string query, object param)
        {
            try
            {
                await CheckConnectionAsync();
                var result = (await _connection.QueryAsync<T>(query, param)).ToList();
                CloseAsync();
                return result;
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return null;
            }

        }

        protected async Task ExecuteAsync(string query, object param)
        {
            try
            {
                await CheckConnectionAsync();
                await _connection.ExecuteAsync(query, param);
                CloseAsync();
            }
            catch (Exception e)
            { 
                Logger.LogException(e);
            }


        }
    }
}