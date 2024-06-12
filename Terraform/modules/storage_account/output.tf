output "primary_connection_string" {
  value     = var.site == "primary" ? azurerm_storage_account.storage_account.primary_connection_string : null
  sensitive = true
}