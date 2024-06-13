######################################################
##                     Project                      ##
######################################################
locals {
  tags = {
    "Managed"     = "Terraform"
    "Project"     = var.project
    "Environment" = var.environment
    "Location"    = var.location
  }
}

######################################################
##                Naming Convention                 ##
######################################################
locals {
  name_suffix                                    = "${var.project}-${var.environment}-${var.location}"
  resource_group_name                            = "rg-${local.name_suffix}"
  virtual_network_name                           = "vnet-${local.name_suffix}"
  network_security_group_names                   = { for nsg in var.network_security_group_names : nsg => "nsg-${replace(local.name_suffix, var.project, nsg)}" }
  log_analytics_workspace_name                   = "log-${local.name_suffix}"
  linux_virtual_machine_name                     = "vm-${replace(local.name_suffix, var.project, "linux")}"
  linux_virtual_machine_network_interface_name   = "nic-${replace(local.name_suffix, var.project, "linux")}"
  linux_virtual_machine_os_disk_name             = "osdisk-${replace(local.name_suffix, var.project, "linux")}"
  windows_virtual_machine_name                   = "vm-${replace(local.name_suffix, var.project, "windows")}"
  windows_virtual_machine_network_interface_name = "nic-${replace(local.name_suffix, var.project, "windows")}"
  windows_virtual_machine_os_disk_name           = "osdisk-${replace(local.name_suffix, var.project, "windows")}"
  container_registry_name                        = "cr${replace(local.name_suffix, "-", "")}"
  redis_cache_name                               = "redis-${local.name_suffix}"
  identity_name                                  = "id-${local.name_suffix}"
  public_ip_name                                 = "pip-${local.name_suffix}"
  bastion_host_name                              = "bas-${local.name_suffix}"
  bas_public_ip_name                             = "pip-${replace(local.name_suffix, var.project, "bas")}"
  bas_ip_configuration_name                      = "basip-${local.name_suffix}"
  service_plan_name                              = "asp-${local.name_suffix}"
  azurerm_mssql_server_name                      = "sql-${local.name_suffix}"
  mssql_database_name                            = "sqldb-${local.name_suffix}"
  private_link_service_name                      = "pl-${local.name_suffix}"
  custom_network_interface_name_mssql_server     = "nic-${replace(local.name_suffix, var.project, "sqlServer")}"
  private_endpoint_name_mssql_server             = "pep-${replace(local.name_suffix, var.project, "sqlServer")}"
  api_management_names                           = { for apim in var.api_management_names : apim => "apim-${replace(local.name_suffix, var.project, apim)}" }
  subnets = [
    for s in var.subnets : {
      name           = s.name == "AzureBastionSubnet" ? s.name : "snet-${s.name}-${local.name_suffix}"
      address_prefix = s.address_prefix
    }
  ]
}
