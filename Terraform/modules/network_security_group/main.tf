######################################################
##              Network Security Group              ##
######################################################
resource "azurerm_network_security_group" "network_security_group" {
  for_each            = var.network_security_group_names
  name                = each.value
  location            = var.location
  resource_group_name = var.resource_group_name
  tags                = var.tags
}


