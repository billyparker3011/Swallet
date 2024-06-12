######################################################
##                     Project                      ##
######################################################
variable "resource_group_name" {
  type        = string
  description = "(Required) The name of the resource group in which to create the storage account. Changing this forces a new resource to be created."
}

variable "location" {
  type        = string
  description = "(Required) Specifies the supported Azure location where the resource exists. Changing this forces a new resource to be created."
}

variable "tags" {
  type        = map(string)
  description = "(Optional) A mapping of tags to assign to the resource."
}

######################################################
##                    Redis Cache                   ##
######################################################
variable "name" {
  type        = string
  description = "(Required) The name of the Redis instance. Changing this forces a new resource to be created."
}

variable "capacity" {
  type        = number
  description = "(Required) The size of the Redis cache to deploy. Valid values for a SKU family of C (Basic/Standard) are 0, 1, 2, 3, 4, 5, 6, and for P (Premium) family are 1, 2, 3, 4, 5."
}

variable "family" {
  type        = string
  description = "(Required) The SKU family/pricing group to use. Valid values are C (for Basic/Standard SKU family) and P (for Premium)"
}

variable "sku_name" {
  type        = string
  description = "(Required) The SKU of Redis to use. Possible values are Basic, Standard and Premium."
}

variable "minimum_tls_version" {
  type        = string
  description = "(Optional) The minimum TLS version. Possible values are 1.0, 1.1 and 1.2. Defaults to 1.0."
}

variable "identity_type" {
  type        = string
  description = "(Required) Specifies the type of Managed Service Identity that should be configured on this Redis Cluster. Possible values are SystemAssigned, UserAssigned, SystemAssigned, UserAssigned (to enable both)."
  default     = "UserAssigned"
}

variable "identity_ids" {
  type        = list(string)
  description = "(Optional) A list of User Assigned Managed Identity IDs to be assigned to this Redis Cluster."
}

variable "enable_non_ssl_port" {
  type        = bool
  description = "(Optional) Enable the non-SSL port (6379) - disabled by default."
  default     = true
}
