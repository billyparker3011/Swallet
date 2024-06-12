######################################################
##                 Public IP Address                ##
######################################################
resource "azurerm_public_ip" "bastion_host" {
  name                = var.bas_public_ip_name
  resource_group_name = var.resource_group_name
  location            = var.location
  allocation_method   = var.allocation_method
  sku                 = var.public_ip_sku
  tags                = var.tags
}

######################################################
##                   Bastion Host                   ##
######################################################
resource "azurerm_bastion_host" "bastion_host" {
  name                   = var.bastion_host_name
  resource_group_name    = var.resource_group_name
  location               = var.location
  sku                    = var.sku
  shareable_link_enabled = var.shareable_link_enabled
  tunneling_enabled      = var.tunneling_enabled
  tags                   = var.tags

  ip_configuration {
    name                 = var.bas_ip_configuration_name
    subnet_id            = var.bas_ip_configuration_subnet_id
    public_ip_address_id = azurerm_public_ip.bastion_host.id
  }
}
