######################################################
##            MSSQL Azure Database Server           ##
######################################################
data "azurerm_client_config" "current" {}

resource "azurerm_mssql_server" "mssql_server" {
  name                          = var.azurerm_mssql_server_name
  resource_group_name           = var.resource_group_name
  location                      = var.location
  version                       = var.azurerm_mssql_server_version
  administrator_login           = var.administrator_login
  administrator_login_password  = var.administrator_login_password
  minimum_tls_version           = var.minimum_tls_version
  public_network_access_enabled = var.public_network_access_enabled
  tags                          = var.tags
}

######################################################
##              Azure SQL Firewall Rule             ##
######################################################
resource "azurerm_mssql_firewall_rule" "mssql_firewall_rule" {
  for_each         = { for firewall_rule in var.mssql_firewall_rules : firewall_rule.name => firewall_rule }
  name             = each.value.name
  server_id        = azurerm_mssql_server.mssql_server.id
  start_ip_address = each.value.start_ip_address
  end_ip_address   = each.value.end_ip_address
}

######################################################
##                  MSSQL Database                  ##
######################################################
resource "azurerm_mssql_database" "mssql_database" {
  name         = var.mssql_database_name
  server_id    = azurerm_mssql_server.mssql_server.id
  license_type = var.license_type
  max_size_gb  = var.sqldb_max_size_gb
  sku_name     = var.sqldb_sku_name
  enclave_type = var.enclave_type
  tags         = var.tags

  #This block is used to prevent Terraform from reverting any changes made by Helm
  lifecycle {
    ignore_changes = [
      sku_name,
      max_size_gb
    ]
  }
}

######################################################
##                 Private DNS zone                 ##
######################################################
resource "azurerm_private_dns_zone" "private_dns_zone" {
  name                = var.private_dns_zone_name
  resource_group_name = var.resource_group_name
  tags                = var.tags
}

resource "azurerm_private_dns_zone_virtual_network_link" "private_dns_zone_virtual_network_link" {
  name                  = var.private_dns_zone_virtual_network_link_name
  resource_group_name   = var.resource_group_name
  private_dns_zone_name = azurerm_private_dns_zone.private_dns_zone.name
  virtual_network_id    = var.virtual_network_id
  tags                  = var.tags
}

######################################################
##                 Private Endpoint                 ##
######################################################
resource "azurerm_private_endpoint" "primary_private_endpoint" {
  name                          = var.private_endpoint_name
  location                      = var.location
  resource_group_name           = var.resource_group_name
  subnet_id                     = var.subnet_id
  custom_network_interface_name = var.custom_network_interface_name

  private_dns_zone_group {
    name                 = var.private_dns_zone_group_name
    private_dns_zone_ids = [azurerm_private_dns_zone.private_dns_zone.id]
  }

  private_service_connection {
    name                           = var.private_service_connection_name
    is_manual_connection           = var.private_service_connection_is_manual_connection
    private_connection_resource_id = azurerm_mssql_server.mssql_server.id
    subresource_names              = var.private_service_connection_subresource_names
  }
}