######################################################
##                   Linux Web App                  ##
######################################################
resource "azurerm_linux_web_app" "development_linux_web_app" {
  for_each            = { for linux_web_app in var.linux_web_apps : linux_web_app.name => linux_web_app }
  name                = each.value.name
  resource_group_name = var.resource_group_name
  location            = var.location
  service_plan_id     = var.development_service_plan_id
  tags                = var.tags

  app_settings = each.value.app_settings

  site_config {
    container_registry_managed_identity_client_id = var.container_registry_managed_identity_client_id
    container_registry_use_managed_identity       = var.container_registry_use_managed_identity

    application_stack {
      docker_image_name   = each.value.docker_image_name
      docker_registry_url = "https://${var.docker_registry_url}"
    }

    cors {
      allowed_origins     = var.cors_allowed_origins
      support_credentials = var.support_credentials
    }
  }

  identity {
    type         = var.type
    identity_ids = var.identity_ids
  }

  lifecycle {
    ignore_changes = [
      site_config[0].application_stack[0].docker_image_name,
      app_settings
    ]
  }
}

