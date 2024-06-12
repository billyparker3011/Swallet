######################################################
##                 Virtual Network                  ##
######################################################
resource "azurerm_virtual_network" "virtual_network" {
  name                = var.virtual_network_name
  location            = var.location
  resource_group_name = var.resource_group_name
  address_space       = var.address_space
  tags                = var.tags
}

######################################################
##                      Subnet                      ##
######################################################
resource "azurerm_subnet" "subnet" {
  for_each             = { for s in var.subnets : s.name => s }
  name                 = each.value.name
  resource_group_name  = var.resource_group_name
  virtual_network_name = azurerm_virtual_network.virtual_network.name
  address_prefixes     = [each.value.address_prefix]
  service_endpoints    = var.service_endpoints
}
