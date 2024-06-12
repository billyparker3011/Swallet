######################################################
##                     Project                      ##
######################################################
variable "resource_group_name" {
  type        = string
  description = "(Required) The name of the resource group in which to create the Bastion Host. Changing this forces a new resource to be created."
}

variable "location" {
  type        = string
  description = "(Required) Specifies the supported Azure location where the resource exists. Changing this forces a new resource to be created. Review Azure Bastion Host FAQ for supported locations."
}

variable "tags" {
  type        = map(string)
  description = "(Optional) A mapping of tags to assign to the resource."
}

######################################################
##                 Public IP Address                ##
######################################################
variable "bas_public_ip_name" {
  type        = string
  description = "(Required) Specifies the name of the Public IP. Changing this forces a new Public IP to be created."
}

variable "allocation_method" {
  type        = string
  description = "(Required) Specifies the supported Azure location where the Public IP should exist. Changing this forces a new resource to be created."
  default     = "Static"
}

variable "public_ip_sku" {
  type        = string
  description = "(Optional) The SKU of the Public IP. Accepted values are Basic and Standard. Defaults to Basic. Changing this forces a new resource to be created."
  default     = "Standard"
}

######################################################
##                   Bastion Host                   ##
######################################################
variable "bastion_host_name" {
  type        = string
  description = "(Required) Specifies the name of the Bastion Host. Changing this forces a new resource to be created."
}

variable "sku" {
  type        = string
  description = "(Optional) The SKU of the Bastion Host. Accepted values are Basic and Standard. Defaults to Basic."
  default     = "Standard"
}

variable "shareable_link_enabled" {
  type        = bool
  description = "(Optional) Is Shareable Link feature enabled for the Bastion Host. Defaults to false."
  default     = false
}

variable "tunneling_enabled" {
  type        = bool
  description = "(Optional) Is Tunneling feature enabled for the Bastion Host. Defaults to false."
  default     = true
}

variable "bas_ip_configuration_name" {
  type        = string
  description = "(Required) The name of the IP configuration. Changing this forces a new resource to be created."
}

variable "bas_ip_configuration_subnet_id" {
  type        = string
  description = "(Required) Reference to a subnet in which this Bastion Host has been created. Changing this forces a new resource to be created."
}











