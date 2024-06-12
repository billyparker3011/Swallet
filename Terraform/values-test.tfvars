######################################################
##                      Project                     ##
######################################################
project     = "hnx"
environment = "test"
location    = "__location__"
site        = "__site__"

######################################################
##              API Management Service              ##
######################################################
api_management_names    = ["agent", "player"]
api_management_sku_name = "Developer_1"

######################################################
##                   App Service                    ##
######################################################
linux_web_apps = [
  {
    name              = "app-hnx-agent-portal-test-southeastasia"
    docker_image_name = "agent-portal"
    app_settings      = {}
  },
  {
    name              = "app-hnx-agent-test-southeastasia"
    docker_image_name = "agent"
    app_settings = {
      ASPNETCORE_ENVIRONMENT = "Development"
    }
  },
  {
    name              = "app-hnx-agent-authentication-test-southeastasia"
    docker_image_name = "agent-authentication"
    app_settings = {
      ASPNETCORE_ENVIRONMENT = "Development"
    }
  },
  {
    name              = "app-hnx-odd-test-southeastasia"
    docker_image_name = "odd"
    app_settings = {
      ASPNETCORE_ENVIRONMENT = "Development"
    }
  },
  {
    name              = "app-hnx-player-test-southeastasia"
    docker_image_name = "player"
    app_settings = {
      ASPNETCORE_ENVIRONMENT = "Development"
    }
  },
  {
    name              = "app-hnx-player-authentication-test-southeastasia"
    docker_image_name = "player-authentication"
    app_settings = {
      ASPNETCORE_ENVIRONMENT = "Development"
    }
  },
  {
    name              = "app-hnx-player-portal-test-southeastasia"
    docker_image_name = "player-portal"
    app_settings      = {}
  },
  {
    name              = "app-hnx-ticket-test-southeastasia"
    docker_image_name = "ticket"
    app_settings = {
      ASPNETCORE_ENVIRONMENT = "Development"
    }
  },
  {
    name              = "app-hnx-match-test-southeastasia"
    docker_image_name = "match"
    app_settings = {
      ASPNETCORE_ENVIRONMENT = "Development"
    }
  },
  {
    name              = "app-hnx-log-test-southeastasia"
    docker_image_name = "log"
    app_settings = {
      ASPNETCORE_ENVIRONMENT = "Development"
    }
  }
]

######################################################
##                   Bastion Host                   ##
######################################################
shareable_link_enabled = true

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
address_space = ["172.16.0.0/16"]

######################################################
##                      Subnet                      ##
######################################################
subnets = [
  {
    name           = "vm"
    address_prefix = "172.16.0.0/22"
  },
  {
    name           = "pep"
    address_prefix = "172.16.4.0/22"
  },
  {
    name           = "AzureBastionSubnet"
    address_prefix = "172.16.8.0/22"
  },
]

