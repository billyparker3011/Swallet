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
##              API Management Service              ##
######################################################
variable "api_management_names" {
  type        = map(string)
  description = "(Required) The name of the API Management Service. Changing this forces a new resource to be created."
}

variable "sku_name" {
  type        = string
  description = "(Required) sku_name is a string consisting of two parts separated by an underscore(_). The first part is the name, valid values include: Consumption, Developer, Basic, Standard and Premium. The second part is the capacity (e.g. the number of deployed units of the sku), which must be a positive integer (e.g. Developer_1)."
}

variable "publisher_name" {
  type        = string
  description = "(Required) The name of publisher/company."
  default     = "HVN"

}

variable "publisher_email" {
  type        = string
  description = "(Required) The email of publisher/company."
  default     = "hvn@mail.com"
}

