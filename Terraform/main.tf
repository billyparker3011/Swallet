######################################################
##                Backend & Provider                ##
######################################################
# The backend configuration is defined in the pipeline task, so the backend configuration block below is not needed.
terraform {
  backend "azurerm" {
    resource_group_name  = "resource_group_name"
    storage_account_name = "storage_account_name"
    container_name       = "container_name"
    key                  = "key"
  }
}

provider "azurerm" {
  features {
    key_vault {
      purge_soft_delete_on_destroy    = true
      recover_soft_deleted_key_vaults = true
    }
  }
}

######################################################
##              API Management Service              ##
######################################################
module "api_management" {
  source               = "./modules/api_management"
  api_management_names = local.api_management_names
  resource_group_name  = module.resource_group.name
  location             = var.location
  sku_name             = var.api_management_sku_name
  tags                 = local.tags
}


######################################################
##                   Bastion Host                   ##
######################################################
module "bastion_host" {
  source                         = "./modules/bastion_host"
  bas_public_ip_name             = local.bas_public_ip_name
  resource_group_name            = module.resource_group.name
  location                       = var.location
  bastion_host_name              = local.bastion_host_name
  bas_ip_configuration_name      = local.bas_ip_configuration_name
  bas_ip_configuration_subnet_id = module.virtual_network.bas_subnet_id
  shareable_link_enabled         = var.shareable_link_enabled
  tags                           = local.tags
}

######################################################
##                Container Registry                ##
######################################################
module "container_registry" {
  source                  = "./modules/container_registry"
  container_registry_name = local.container_registry_name
  location                = var.location
  resource_group_name     = module.resource_group.name
  tags                    = local.tags
}

######################################################
##                   Linux Web App                  ##
######################################################
module "linux_web_app" {
  source                                        = "./modules/linux_web_app"
  linux_web_apps                                = var.linux_web_apps
  location                                      = var.location
  resource_group_name                           = module.resource_group.name
  development_service_plan_id                   = module.service_plan.development_service_plan_id
  docker_registry_url                           = module.container_registry.login_server
  container_registry_managed_identity_client_id = module.user_assigned_identity.user_assigned_identity_client_id[0]
  identity_ids                                  = module.user_assigned_identity.user_assigned_identity_id
  environment                                   = var.environment
  tags                                          = local.tags
}

######################################################
##               Log Analytics Workspace            ##
######################################################
module "log_analytics_workspace" {
  source              = "./modules/log_analytics_workspace"
  location            = var.location
  name                = local.log_analytics_workspace_name
  resource_group_name = module.resource_group.name
  tags                = local.tags
}

######################################################
##            MSSQL Azure Database Server           ##
######################################################
module "mssql_server" {
  source                                     = "./modules/mssql_server"
  azurerm_mssql_server_name                  = local.azurerm_mssql_server_name
  resource_group_name                        = module.resource_group.name
  location                                   = var.location
  administrator_login                        = var.administrator_login
  administrator_login_password               = module.random_password.mssql_server
  public_network_access_enabled              = var.public_network_access_enabled
  mssql_firewall_rules                       = var.mssql_firewall_rules
  mssql_database_name                        = local.mssql_database_name
  sqldb_max_size_gb                          = var.sqldb_max_size_gb
  sqldb_sku_name                             = var.sqldb_sku_name
  private_dns_zone_virtual_network_link_name = local.virtual_network_name
  virtual_network_id                         = module.virtual_network.virtual_network_id
  custom_network_interface_name              = local.custom_network_interface_name_mssql_server
  private_dns_zone_group_name                = local.private_link_service_name
  private_endpoint_name                      = local.private_endpoint_name_mssql_server
  private_service_connection_name            = local.private_link_service_name
  subnet_id                                  = module.virtual_network.pep_subnet_id
  site                                       = var.site
  tags                                       = local.tags
}

######################################################
##              Network Security Group              ##
######################################################
module "network_security_group" {
  source                       = "./modules/network_security_group"
  location                     = var.location
  network_security_group_names = local.network_security_group_names
  resource_group_name          = module.resource_group.name
  tags                         = local.tags
}

######################################################
##                  Random Password                 ##
######################################################
module "random_password" {
  source = "./modules/random_password"
}

######################################################
##                    Redis Cache                   ##
######################################################
module "redis_cache" {
  source              = "./modules/redis_cache"
  capacity            = var.redis_cache_capacity
  family              = var.redis_cache_family
  identity_ids        = module.user_assigned_identity.user_assigned_identity_id
  location            = var.location
  minimum_tls_version = var.redis_cache_minimum_tls_version
  name                = local.redis_cache_name
  resource_group_name = local.resource_group_name
  sku_name            = var.redis_cache_sku_name
  tags                = local.tags
}

######################################################
##                  Resource Group                  ##
######################################################
module "resource_group" {
  source              = "./modules/resource_group"
  location            = var.location
  resource_group_name = local.resource_group_name
  tags                = local.tags
}

######################################################
##               App Service Plan                   ##
######################################################
module "service_plan" {
  source                            = "./modules/service_plan"
  location                          = var.location
  resource_group_name               = module.resource_group.name
  service_plan_name                 = local.service_plan_name
  development_service_plan_sku_name = var.development_service_plan_sku_name
  environment                       = var.environment
  tags                              = local.tags
}

######################################################
##              User Assigned Identity              ##
######################################################
module "user_assigned_identity" {
  source              = "./modules/user_assigned_identity"
  location            = var.location
  resource_group_name = module.resource_group.name
  identity_name       = local.identity_name
  tags                = local.tags
}

######################################################
##                  Virtual Machine                 ##
######################################################
module "virtual_machine" {
  source                                         = "./modules/virtual_machine"
  resource_group_name                            = module.resource_group.name
  location                                       = var.location
  admin_password                                 = var.admin_password
  computer_name                                  = var.computer_name
  windows_virtual_machine_size                   = var.windows_virtual_machine_size
  size                                           = var.virtual_machine_size
  admin_username                                 = var.admin_username
  linux_virtual_machine_name                     = local.linux_virtual_machine_name
  linux_virtual_machine_network_interface_name   = local.linux_virtual_machine_network_interface_name
  linux_virtual_machine_os_disk_name             = local.linux_virtual_machine_os_disk_name
  linux_virtual_machine_os_disk_size_gb          = var.linux_virtual_machine_os_disk_size_gb
  windows_virtual_machine_name                   = local.windows_virtual_machine_name
  windows_virtual_machine_network_interface_name = local.windows_virtual_machine_network_interface_name
  windows_virtual_machine_os_disk_name           = local.windows_virtual_machine_os_disk_name
  network_interface_ip_configuration_subnet_id   = module.virtual_network.vm_subnet_id
  identity_ids                                   = module.user_assigned_identity.user_assigned_identity_id
  tags                                           = local.tags
}

######################################################
##                  Virtual Network                 ##
######################################################
module "virtual_network" {
  source               = "./modules/virtual_network"
  location             = var.location
  address_space        = var.address_space
  resource_group_name  = module.resource_group.name
  virtual_network_name = local.virtual_network_name
  subnets              = local.subnets
  tags                 = local.tags
}
