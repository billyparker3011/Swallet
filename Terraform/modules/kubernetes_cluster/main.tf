######################################################
##             Azure Kubernetes Service             ##
######################################################
resource "azurerm_kubernetes_cluster" "kubernetes_cluster" {
  name                       = var.kubernetes_cluster_name
  location                   = var.location
  resource_group_name        = var.resource_group_name
  dns_prefix_private_cluster = var.kubernetes_cluster_name
  kubernetes_version         = var.kubernetes_version
  private_cluster_enabled    = var.private_cluster_enabled
  node_resource_group        = var.node_resource_group
  private_dns_zone_id        = var.private_dns_zone_id
  sku_tier                   = var.sku_tier
  azure_policy_enabled       = var.azure_policy_enabled
  tags                       = var.tags

  network_profile {
    network_plugin = var.kubernetes_cluster_network_plugin
  }

  default_node_pool {
    name                        = var.aks_default_node_pool_name
    vm_size                     = var.aks_default_node_pool_vm_size
    enable_auto_scaling         = var.aks_default_node_pool_enable_auto_scaling
    max_pods                    = var.aks_default_node_pool_max_pods
    min_count                   = var.aks_default_node_pool_min_count
    max_count                   = var.aks_default_node_pool_max_count
    vnet_subnet_id              = var.aks_default_node_pool_vnet_subnet_id
    temporary_name_for_rotation = var.aks_default_node_pool_temporary_name_for_rotation
    tags                        = var.tags
  }

  ingress_application_gateway {
    gateway_id = var.aks_ingress_application_gateway_appgw_id
  }

  identity {
    type         = var.aks_identity_type
    identity_ids = var.aks_identity_ids
  }

  key_vault_secrets_provider {
    secret_rotation_enabled = true
  }

  monitor_metrics {}

  oms_agent {
    log_analytics_workspace_id      = var.log_analytics_workspace_id
    msi_auth_for_monitoring_enabled = var.msi_auth_for_monitoring_enabled
  }

  lifecycle {
    ignore_changes = [
      microsoft_defender
    ]
  }
}
