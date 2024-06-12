######################################################
##                     Project                      ##
######################################################
variable "resource_group_name" {
  type        = string
  description = "(Required) The name of the resource group in which to create the virtual network. Changing this forces a new resource to be created."
}

variable "location" {
  type        = string
  description = "(Required) The location/region where the virtual network is created. Changing this forces a new resource to be created."
}

variable "tags" {
  type        = map(string)
  description = "Tags to apply to the virtual network and its subnets"
}

######################################################
##              Network Security Group              ##
######################################################
variable "network_security_group_names" {
  type        = map(string)
  description = "(Required) Specifies the name of the network security group. Changing this forces a new resource to be created."
}
