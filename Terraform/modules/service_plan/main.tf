######################################################
##             App Service: Service Plan            ##
######################################################
resource "azurerm_service_plan" "development_service_plan" {
  count               = var.environment == "dev" || var.environment == "test" ? 1 : 0
  name                = var.service_plan_name
  resource_group_name = var.resource_group_name
  location            = var.location
  os_type             = var.os_type
  sku_name            = var.development_service_plan_sku_name
  tags                = var.tags
}
