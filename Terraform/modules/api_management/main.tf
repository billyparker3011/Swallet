######################################################
##              API Management Service              ##
######################################################
resource "azurerm_api_management" "api_management" {
  for_each            = var.api_management_names
  name                = each.value
  location            = var.location
  resource_group_name = var.resource_group_name
  sku_name            = var.sku_name
  publisher_name      = var.publisher_name
  publisher_email     = var.publisher_email
  tags                = var.tags
}
