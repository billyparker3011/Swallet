######################################################
##                     Project                      ##
######################################################
variable "resource_group_name" {
  type        = string
  description = "(Required) The name of the resource group in which to create the Container Registry. Changing this forces a new resource to be created."
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
##                Container Registry                ##
######################################################
variable "container_registry_name" {
  type        = string
  description = "(Required) Specifies the name of the Container Registry. Only Alphanumeric characters allowed. Changing this forces a new resource to be created."
}

variable "container_registry_sku" {
  type        = string
  description = "(Required) The SKU name of the container registry. Possible values are Basic, Standard and Premium."
  default     = "Standard"
}

variable "zone_redundancy_enabled" {
  type        = bool
  description = "(Optional) Whether zone redundancy is enabled for this Container Registry? Changing this forces a new resource to be created. Defaults to false."
  default     = "false"
}

variable "admin_enabled" {
  type        = bool
  description = "(Optional) Specifies whether the admin user is enabled. Defaults to false."
  default     = true
}

