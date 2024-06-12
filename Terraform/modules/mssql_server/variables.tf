######################################################
##                     Project                      ##
######################################################
variable "resource_group_name" {
  type        = string
  description = "(Required) The name of the resource group in which to create the Microsoft SQL Server. Changing this forces a new resource to be created."
}

variable "location" {
  type        = string
  description = "(Required) The name of the resource group in which to create the Microsoft SQL Server. Changing this forces a new resource to be created."
}

variable "site" {
  type        = string
  description = "(Required) Determine if the region is primary or backup for a Disaster Recovery solution."
}

variable "tags" {
  type        = map(string)
  description = "(Optional) A mapping of tags to assign to the resource."
}

######################################################
##            MSSQL Azure Database Server           ##
######################################################
variable "azurerm_mssql_server_name" {
  type        = string
  description = "(Required) The name of the Microsoft SQL Server. This needs to be globally unique within Azure. Changing this forces a new resource to be created."
}

variable "azurerm_mssql_server_version" {
  type        = string
  description = "(Required) The version for the new server. Valid values are: 2.0 (for v11 server) and 12.0 (for v12 server). Changing this forces a new resource to be created."
  default     = "12.0"
}

variable "administrator_login" {
  type        = string
  description = "(Optional) The administrator login name for the new server. Required unless azuread_authentication_only in the azuread_administrator block is true. When omitted, Azure will generate a default username which cannot be subsequently changed. Changing this forces a new resource to be created."
}

variable "administrator_login_password" {
  type        = string
  description = "(Optional) The password associated with the administrator_login user. Needs to comply with Azure's Password Policy. Required unless azuread_authentication_only in the azuread_administrator block is true."
}

variable "minimum_tls_version" {
  type        = string
  description = "(Optional) The Minimum TLS Version for all SQL Database and SQL Data Warehouse databases associated with the server. Valid values are: 1.0, 1.1 , 1.2 and Disabled. Defaults to 1.2."
  default     = "1.2"
}

variable "public_network_access_enabled" {
  type        = bool
  description = "(Optional) Whether public network access is allowed for this server. Defaults to true."
}

######################################################
##              Azure SQL Firewall Rule             ##
######################################################
variable "mssql_firewall_rules" {
  type = list(object({
    name             = string
    start_ip_address = string
    end_ip_address   = string
  }))
  description = "A list of firewall rules to be created for the Microsoft SQL Server."
}

######################################################
##                  MSSQL Database                  ##
######################################################
variable "mssql_database_name" {
  type        = string
  description = "(Required) The name of the MS SQL Database. Changing this forces a new resource to be created."
}

variable "license_type" {
  type        = string
  description = "(Optional) Specifies the license type applied to this database. Possible values are LicenseIncluded and BasePrice."
  default     = "LicenseIncluded"
}

variable "sqldb_max_size_gb" {
  type        = number
  description = "(Optional) The max size of the database in gigabytes."
}

variable "sqldb_sku_name" {
  type        = string
  description = "(Optional) Specifies the name of the SKU used by the database. For example, GP_S_Gen5_2,HS_Gen4_1,BC_Gen5_2, ElasticPool, Basic,S0, P2 ,DW100c, DS100. Changing this from the HyperScale service tier to another service tier will create a new resource."
}

variable "enclave_type" {
  type        = string
  description = "(Optional) Specifies the type of enclave to be used by the elastic pool. Possible value VBS."
  default     = "VBS"
}

######################################################
##                 Private DNS zone                 ##
######################################################
variable "private_dns_zone_name" {
  type        = string
  description = "(Required) Specifies the Name of the Private DNS Zone Group."
  default     = "privatelink.database.windows.net"
}

variable "private_dns_zone_virtual_network_link_name" {
  type        = string
  description = "(Required) The name of the Private DNS Zone Virtual Network Link. Changing this forces a new resource to be created."
}

variable "virtual_network_id" {
  type        = string
  description = "(Required) The ID of the Virtual Network that should be linked to the DNS Zone. Changing this forces a new resource to be created."
}

######################################################
##                 Private Endpoint                 ##
######################################################
variable "private_endpoint_name" {
  type        = string
  description = "(Required) Specifies the Name of the Private Endpoint. Changing this forces a new resource to be created."
}

variable "subnet_id" {
  type        = string
  description = "(Required) The ID of the Subnet from which Private IP Addresses will be allocated for this Private Endpoint. Changing this forces a new resource to be created."
}

variable "custom_network_interface_name" {
  type        = string
  description = "(Optional) The custom name of the network interface attached to the private endpoint. Changing this forces a new resource to be created."
}

variable "private_dns_zone_group_name" {
  type        = string
  description = "(Required) Specifies the Name of the Private DNS Zone Group."
}

variable "private_service_connection_name" {
  type        = string
  description = "(Required) Specifies the Name of the Private Service Connection. Changing this forces a new resource to be created."
}

variable "private_service_connection_is_manual_connection" {
  type        = bool
  description = "(Required) Does the Private Endpoint require Manual Approval from the remote resource owner? Changing this forces a new resource to be created."
  default     = false
}

variable "private_service_connection_subresource_names" {
  type        = list(string)
  description = "(Optional) A list of subresource names which the Private Endpoint is able to connect to. subresource_names corresponds to group_id. Possible values are detailed in the product documentation in the Subresources column. Changing this forces a new resource to be created."
  default     = ["sqlServer"]
}