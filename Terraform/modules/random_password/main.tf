######################################################
##                  Random Password                 ##
######################################################
resource "random_password" "mssql_server" {
  length = var.length

  lifecycle {
    ignore_changes = [
      length,
    ]
  }
}