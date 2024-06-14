######################################################
##                 Public IP Address                ##
######################################################
resource "azurerm_public_ip" "public_ip" {
  name                = var.public_ip_name
  location            = var.location
  resource_group_name = var.resource_group_name
  allocation_method   = var.allocation_method
  sku                 = var.public_ip_sku
  zones               = var.zones
  tags                = var.tags
}

######################################################
##                Application Gateway               ##
######################################################
resource "azurerm_application_gateway" "application_gateway" {
  name                = var.application_gateway_name
  resource_group_name = var.resource_group_name
  location            = var.location
  enable_http2        = var.enable_http2
  zones               = var.zones
  tags                = var.tags

  sku {
    name = var.agw_sku_name
    tier = var.agw_sku_tier
  }

  autoscale_configuration {
    min_capacity = var.agw_autoscale_configuration_min_capacity
    max_capacity = var.agw_autoscale_configuration_max_capacity
  }

  gateway_ip_configuration {
    name      = var.agw_gateway_ip_configuration_name
    subnet_id = var.agw_subnet_id
  }

  frontend_port {
    name = var.agw_frontend_port_name
    port = var.agw_frontend_port
  }

  frontend_ip_configuration {
    name                 = var.agw_frontend_ip_configuration_name
    public_ip_address_id = azurerm_public_ip.public_ip.id
  }

  backend_address_pool {
    name = var.agw_backend_address_pool_name
  }

  backend_http_settings {
    name                  = var.agw_backend_http_settings_name
    cookie_based_affinity = var.agw_backend_http_settings_cookie_based_affinity
    port                  = var.agw_backend_http_settings_port
    protocol              = var.agw_backend_http_settings_protocol
  }

  http_listener {
    name                           = var.agw_http_listener_name
    frontend_ip_configuration_name = var.agw_frontend_ip_configuration_name
    frontend_port_name             = var.agw_frontend_port_name
    protocol                       = var.agw_http_listener_protocol
  }

  request_routing_rule {
    name                       = var.agw_request_routing_rule_name
    rule_type                  = var.agw_request_routing_rule_rule_type
    http_listener_name         = var.agw_http_listener_name
    backend_address_pool_name  = var.agw_backend_address_pool_name
    backend_http_settings_name = var.agw_backend_http_settings_name
    priority                   = var.agw_request_routing_rule_priority
  }

  ssl_policy {
    min_protocol_version = var.agw_ssl_policy_min_protocol_version
    policy_name          = var.agw_ssl_policy_name
    policy_type          = var.agw_ssl_policy_type
  }

  #This block is used to prevent Terraform from reverting any changes made by Helm
  lifecycle {
    ignore_changes = [
      backend_address_pool,
      frontend_port,
      redirect_configuration,
      backend_http_settings,
      http_listener,
      request_routing_rule,
      ssl_certificate,
      probe,
      url_path_map,
      ssl_policy,
      tags,
      frontend_port,
      redirect_configuration
    ]
  }
}