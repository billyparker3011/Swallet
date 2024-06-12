######################################################
##                     Project                      ##
######################################################
variable "resource_group_name" {
  type        = string
  description = "(Required) The name of the Resource Group in which the Linux Virtual Machine should be exist. Changing this forces a new resource to be created."
}

variable "location" {
  type        = string
  description = "(Required) The Azure location where the Linux Virtual Machine should exist. Changing this forces a new resource to be created."
}

variable "tags" {
  type        = map(string)
  description = "(Optional) A mapping of tags which should be assigned to this Virtual Machine."
}

######################################################
##                 Network Interface                ##
######################################################
variable "linux_virtual_machine_network_interface_name" {
  type        = string
  description = "(Required) The name of the Network Interface. Changing this forces a new resource to be created."
}

variable "network_interface_ip_configuration_subnet_id" {
  type        = string
  description = "(Optional) The ID of the Subnet where this Network Interface should be located in."
}

variable "network_interface_ip_configuration_private_ip_address_allocation" {
  type        = string
  description = "(Required) The allocation method used for the Private IP Address. Possible values are Dynamic and Static."
  default     = "Dynamic"
}

######################################################
##                  Virtual Machine                 ##
######################################################
variable "size" {
  type        = string
  description = "(Required) The SKU which should be used for this Virtual Machine, such as Standard_F2."
}

variable "admin_username" {
  type        = string
  description = "(Required) The username of the local administrator used for the Virtual Machine. Changing this forces a new resource to be created."
}

variable "source_image_reference_version" {
  type        = string
  description = "(Required) Specifies the version of the image used to create the virtual machines. Changing this forces a new resource to be created."
  default     = "latest"
}

variable "os_disk_storage_account_type" {
  type        = string
  description = "(Required) The Type of Storage Account which should back this the Internal OS Disk. Possible values are Standard_LRS, StandardSSD_LRS, Premium_LRS, StandardSSD_ZRS and Premium_ZRS. Changing this forces a new resource to be created."
  default     = "Standard_LRS"
}

variable "os_disk_caching" {
  type        = string
  description = "(Required) The Type of Caching which should be used for the Internal OS Disk. Possible values are None, ReadOnly and ReadWrite."
  default     = "ReadWrite"
}

variable "linux_virtual_machine_os_disk_size_gb" {
  type        = number
  description = "(Optional) The Size of the Internal OS Disk in GB, if you wish to vary from the size used in the image this Virtual Machine is sourced from."
}

variable "identity_type" {
  type        = string
  description = "(Required) Specifies the type of Managed Service Identity that should be configured on this Linux Virtual Machine. Possible values are SystemAssigned, UserAssigned, SystemAssigned, UserAssigned (to enable both)."
  default     = "UserAssigned"
}

variable "identity_ids" {
  type        = list(string)
  description = "(Optional) Specifies a list of User Assigned Managed Identity IDs to be assigned to this Linux Virtual Machine."
}

######################################################
##               Linux Virtual Machine              ##
######################################################
variable "linux_virtual_machine_name" {
  type        = string
  description = "(Required) The name of the Linux Virtual Machine. Changing this forces a new resource to be created."
}

variable "provision_vm_agent" {
  type        = bool
  description = "(Optional) Should the Azure VM Agent be provisioned on this Virtual Machine? Defaults to true. Changing this forces a new resource to be created."
  default     = true
}

variable "linux_virtual_machine_source_image_reference_publisher" {
  type        = string
  description = "(Required) Specifies the publisher of the image used to create the virtual machines. Changing this forces a new resource to be created."
  default     = "canonical"
}

variable "linux_virtual_machine_source_image_reference_offer" {
  type        = string
  description = "(Required) Specifies the offer of the image used to create the virtual machines. Changing this forces a new resource to be created."
  default     = "0001-com-ubuntu-server-focal"
}

variable "linux_virtual_machine_source_image_reference_sku" {
  type        = string
  description = "(Required) Specifies the SKU of the image used to create the virtual machines. Changing this forces a new resource to be created."
  default     = "20_04-lts"
}

variable "linux_virtual_machine_os_disk_name" {
  type        = string
  description = "(Optional) The name which should be used for the Internal OS Disk. Changing this forces a new resource to be created."
}


######################################################
##              Windows Virtual Machine             ##
######################################################
variable "windows_virtual_machine_name" {
  type        = string
  description = "(Required) The name of the Windows Virtual Machine. Changing this forces a new resource to be created."
}

variable "admin_password" {
  type        = string
  description = "(Required) The Password which should be used for the local-administrator on this Virtual Machine. Changing this forces a new resource to be created."
}

variable "computer_name" {
  type        = string
  description = "(Optional) Specifies the Hostname which should be used for this Virtual Machine. If unspecified this defaults to the value for the name field. If the value of the name field is not a valid computer_name, then you must specify computer_name. Changing this forces a new resource to be created."
}

variable "windows_virtual_machine_os_disk_name" {
  type        = string
  description = "(Optional) The name which should be used for the Internal OS Disk. Changing this forces a new resource to be created."
}

variable "windows_virtual_machine_source_image_reference_publisher" {
  type        = string
  description = "(Required) Specifies the publisher of the image used to create the virtual machines. Changing this forces a new resource to be created."
  default     = "MicrosoftWindowsServer"
}

variable "windows_virtual_machine_source_image_reference_offer" {
  type        = string
  description = "(Required) Specifies the offer of the image used to create the virtual machines. Changing this forces a new resource to be created."
  default     = "WindowsServer"
}

variable "windows_virtual_machine_source_image_reference_sku" {
  type        = string
  description = "(Required) Specifies the SKU of the image used to create the virtual machines. Changing this forces a new resource to be created."
  default     = "2019-datacenter-gensecond"
}

variable "windows_virtual_machine_network_interface_name" {
  type        = string
  description = "(Required) The name of the Network Interface. Changing this forces a new resource to be created."
}

variable "windows_virtual_machine_size" {
  type        = string
  description = "(Required) The SKU which should be used for this Virtual Machine, such as Standard_F2."
}