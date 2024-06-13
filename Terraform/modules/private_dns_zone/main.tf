resource "azurerm_private_dns_zone" "private_dns_zone" {
  name                = "privatelink.${var.location}.azmk8s.io"
  resource_group_name = var.resource_group_name
  tags                = var.tags
}

resource "azurerm_private_dns_zone_virtual_network_link" "private_dns_zone_virtual_network_link" {
  name                  = var.private_dns_zone_virtual_network_link_name
  resource_group_name   = var.resource_group_name
  private_dns_zone_name = azurerm_private_dns_zone.private_dns_zone.name
  virtual_network_id    = var.private_dns_zone_virtual_network_link_virtual_network_id
  tags                  = var.tags
}