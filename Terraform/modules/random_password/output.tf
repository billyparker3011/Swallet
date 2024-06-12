output "mssql_server" {
  value = nonsensitive(random_password.mssql_server.result)
}