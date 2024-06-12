######################################################
##                 Container Registry               ##
######################################################
resource "azurerm_container_registry" "container_registry" {
  name                    = var.container_registry_name
  resource_group_name     = var.resource_group_name
  location                = var.location
  sku                     = var.container_registry_sku
  zone_redundancy_enabled = var.zone_redundancy_enabled
  admin_enabled           = var.admin_enabled
  tags                    = var.tags
}