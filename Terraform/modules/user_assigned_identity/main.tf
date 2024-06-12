resource "azurerm_user_assigned_identity" "user_assigned_identity" {
  resource_group_name = var.resource_group_name
  location            = var.location
  name                = var.identity_name
  tags                = var.tags
}