######################################################
##                     Project                      ##
######################################################
variable "project" {
  type = string
}

variable "environment" {
  type = string
}

variable "location" {
  type = string
}

variable "site" {
  type = string
}

######################################################
##              API Management Service              ##
######################################################
variable "api_management_names" {
  type        = list(string)
  description = "(Required) The name of the API Management Service. Changing this forces a new resource to be created."
}

variable "api_management_sku_name" {
  type        = string
  description = "(Required) sku_name is a string consisting of two parts separated by an underscore(_). The first part is the name, valid values include: Consumption, Developer, Basic, Standard and Premium. The second part is the capacity (e.g. the number of deployed units of the sku), which must be a positive integer (e.g. Developer_1)."
}

######################################################
##                   Bastion Host                   ##
######################################################
variable "shareable_link_enabled" {
  type        = bool
  description = "(Optional) Is Shareable Link feature enabled for the Bastion Host. Defaults to false."
}

######################################################
##                   Linux Web App                  ##
######################################################
variable "linux_web_apps" {
  type = list(object({
    name              = string
    docker_image_name = string
    app_settings      = map(string)
  }))
  description = "(Required) The name which should be used for this Linux Web App. Changing this forces a new Linux Web App to be created."
}

######################################################
##            MSSQL Azure Database Server           ##
######################################################
variable "administrator_login" {
  type        = string
  description = "(Optional) The administrator login name for the new server. Required unless azuread_authentication_only in the azuread_administrator block is true. When omitted, Azure will generate a default username which cannot be subsequently changed. Changing this forces a new resource to be created."
}

variable "public_network_access_enabled" {
  type        = bool
  description = "(Optional) Whether public network access is allowed for this server. Defaults to true."
}

variable "mssql_firewall_rules" {
  type = list(object({
    name             = string
    start_ip_address = string
    end_ip_address   = string
  }))
  description = "A list of firewall rules to be created for the Microsoft SQL Server."
}

variable "sqldb_max_size_gb" {
  type        = number
  description = "(Optional) The max size of the database in gigabytes."
}

variable "sqldb_sku_name" {
  type        = string
  description = "(Optional) Specifies the name of the SKU used by the database. For example, GP_S_Gen5_2,HS_Gen4_1,BC_Gen5_2, ElasticPool, Basic,S0, P2 ,DW100c, DS100. Changing this from the HyperScale service tier to another service tier will create a new resource."
}

######################################################
##              Network Security Group              ##
######################################################
variable "network_security_group_names" {
  type = list(string)
}

######################################################
##                    Redis Cache                   ##
######################################################
variable "redis_cache_capacity" {
  type        = number
  description = "(Required) The size of the Redis cache to deploy. Valid values for a SKU family of C (Basic/Standard) are 0, 1, 2, 3, 4, 5, 6, and for P (Premium) family are 1, 2, 3, 4, 5."
}

variable "redis_cache_family" {
  type        = string
  description = "(Required) The SKU family/pricing group to use. Valid values are C (for Basic/Standard SKU family) and P (for Premium)"
}

variable "redis_cache_sku_name" {
  type        = string
  description = "(Required) The SKU of Redis to use. Possible values are Basic, Standard and Premium."
}

variable "redis_cache_minimum_tls_version" {
  type        = string
  description = "(Optional) The minimum TLS version. Possible values are 1.0, 1.1 and 1.2. Defaults to 1.0."
}

######################################################
##             App Service: Service Plan            ##
######################################################
variable "development_service_plan_sku_name" {
  type        = string
  description = "(Required) The SKU for the plan. Possible values include B1, B2, B3, D1, F1, I1, I2, I3, I1v2, I2v2, I3v2, I4v2, I5v2, I6v2, P1v2, P2v2, P3v2, P0v3, P1v3, P2v3, P3v3, P1mv3, P2mv3, P3mv3, P4mv3, P5mv3, S1, S2, S3, SHARED, EP1, EP2, EP3, WS1, WS2, WS3, and Y1."
}

######################################################
##                  Virtual Machine                 ##
######################################################
variable "virtual_machine_size" {
  type        = string
  description = "(Required) The SKU which should be used for this Virtual Machine, such as Standard_F2."
}

variable "admin_username" {
  type        = string
  description = "(Required) The username of the local administrator used for the Virtual Machine. Changing this forces a new resource to be created."
}

variable "admin_password" {
  type        = string
  description = "(Required) The Password which should be used for the local-administrator on this Virtual Machine. Changing this forces a new resource to be created."
}

variable "computer_name" {
  type        = string
  description = "(Optional) Specifies the Hostname which should be used for this Virtual Machine. If unspecified this defaults to the value for the name field. If the value of the name field is not a valid computer_name, then you must specify computer_name. Changing this forces a new resource to be created."
}

variable "linux_virtual_machine_os_disk_size_gb" {
  type        = number
  description = "(Optional) The Size of the Internal OS Disk in GB, if you wish to vary from the size used in the image this Virtual Machine is sourced from."
}

variable "windows_virtual_machine_size" {
  type        = string
  description = "(Required) The SKU which should be used for this Virtual Machine, such as Standard_F2."
}

######################################################
##                 Virtual Network                  ##
######################################################
variable "address_space" {
  type = list(string)
}

variable "subnets" {
  type = list(object({
    name           = string
    address_prefix = string
  }))
}
