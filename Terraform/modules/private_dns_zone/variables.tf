variable "resource_group_name" {
  type        = string
  description = "(Required) Specifies the resource group where the resource exists. Changing this forces a new resource to be created."
}

variable "location" {
  type        = string
  description = "(Required) The location where the Managed Kubernetes Cluster should be created. Changing this forces a new resource to be created."
}

variable "tags" {
  type        = map(string)
  description = "(Optional) A mapping of tags to assign to the resource."
}

variable "private_dns_zone_virtual_network_link_name" {
  type        = string
  description = "(Required) The name of the Private DNS Zone Virtual Network Link. Changing this forces a new resource to be created."
}

variable "private_dns_zone_virtual_network_link_virtual_network_id" {
  type        = string
  description = "(Required) The ID of the Virtual Network that should be linked to the DNS Zone. Changing this forces a new resource to be created."
}