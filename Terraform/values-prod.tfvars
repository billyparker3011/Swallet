######################################################
##                      Project                     ##
######################################################
project     = "hnx"
environment = "prod"
location    = "__location__"
site        = "__site__"

######################################################
##              API Management Service              ##
######################################################
api_management_names    = ["agent", "player"]
api_management_sku_name = "Premium_1"

######################################################
##                Application Gateway               ##
######################################################
zones = ["1", "2", "3"]

######################################################
##                   Bastion Host                   ##
######################################################
shareable_link_enabled = true

######################################################
##             Azure Kubernetes Service             ##
######################################################
kubernetes_version              = "1.29.4"
aks_default_node_pool_vm_size   = "Standard_D4s_v3"
aks_default_node_pool_max_count = 10
aks_default_node_pool_min_count = 1

######################################################
##            MSSQL Azure Database Server           ##
######################################################
administrator_login           = "hnxadmin"
public_network_access_enabled = true

######################################################
##                  MSSQL Database                  ##
######################################################
sqldb_max_size_gb = 250
sqldb_sku_name    = "S0"

######################################################
##              Azure SQL Firewall Rule             ##
######################################################
mssql_firewall_rules = [
  {
    name             = "Azure services"
    start_ip_address = "0.0.0.0"
    end_ip_address   = "0.0.0.0"
  }
]

######################################################
##              Network Security Group              ##
######################################################
network_security_group_names = ["bas", "vm", "pep"]

######################################################
##                    Redis Cache                   ##
######################################################
redis_cache_capacity            = 2
redis_cache_family              = "C"
redis_cache_sku_name            = "Standard"
redis_cache_minimum_tls_version = "1.2"

######################################################
##               App Service Plan                   ##
######################################################
development_service_plan_sku_name = "B3"

######################################################
##                  Virtual Machine                 ##
######################################################
computer_name                         = "akaHNX"
admin_username                        = "hnxadmin"
admin_password                        = "Qy13743H8q!^"
virtual_machine_size                  = "Standard_D8s_v3"
windows_virtual_machine_size          = "Standard_D4s_v3"
linux_virtual_machine_os_disk_size_gb = 300

######################################################
##                  Virtual Network                 ##
######################################################
address_space = ["10.10.0.0/16"]

######################################################
##                      Subnet                      ##
######################################################
subnets = [
  {
    name           = "vm"
    address_prefix = "10.10.0.0/22"
  },
  {
    name           = "pep"
    address_prefix = "10.10.4.0/22"
  },
  {
    name           = "AzureBastionSubnet"
    address_prefix = "10.10.8.0/22"
  },
  {
    name           = "aks"
    address_prefix = "10.10.12.0/22"
  },
  {
    name           = "agw"
    address_prefix = "10.10.16.0/22"
  },
  {
    name           = "apim"
    address_prefix = "10.10.20.0/22"
  }
]

