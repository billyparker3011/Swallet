######################################################
##                 Network Interface                ##
######################################################
resource "azurerm_network_interface" "linux_virtual_machine_network_interface" {
  name                = var.linux_virtual_machine_network_interface_name
  resource_group_name = var.resource_group_name
  location            = var.location
  tags                = var.tags

  ip_configuration {
    name                          = var.linux_virtual_machine_network_interface_name
    subnet_id                     = var.network_interface_ip_configuration_subnet_id
    private_ip_address_allocation = var.network_interface_ip_configuration_private_ip_address_allocation
  }
}

######################################################
##               Linux Virtual Machine              ##
######################################################
resource "azurerm_linux_virtual_machine" "linux_virtual_machine" {
  name                  = var.linux_virtual_machine_name
  resource_group_name   = var.resource_group_name
  location              = var.location
  size                  = var.size
  admin_username        = var.admin_username
  custom_data           = base64encode(file("${path.module}/cloud-init.yaml"))
  network_interface_ids = [azurerm_network_interface.linux_virtual_machine_network_interface.id]
  provision_vm_agent    = var.provision_vm_agent
  tags                  = var.tags

  admin_ssh_key {
    username   = var.admin_username
    public_key = file("${path.module}/id_rsa.pub")
  }

  source_image_reference {
    publisher = var.linux_virtual_machine_source_image_reference_publisher
    offer     = var.linux_virtual_machine_source_image_reference_offer
    sku       = var.linux_virtual_machine_source_image_reference_sku
    version   = var.source_image_reference_version
  }

  os_disk {
    storage_account_type = var.os_disk_storage_account_type
    caching              = var.os_disk_caching
    name                 = var.linux_virtual_machine_os_disk_name
    disk_size_gb         = var.linux_virtual_machine_os_disk_size_gb
  }

  identity {
    type         = var.identity_type
    identity_ids = var.identity_ids
  }
}

######################################################
##             Windows Network Interface            ##
######################################################
resource "azurerm_network_interface" "windows_virtual_machine_network_interface" {
  name                = var.windows_virtual_machine_network_interface_name
  resource_group_name = var.resource_group_name
  location            = var.location
  tags                = var.tags

  ip_configuration {
    name                          = var.windows_virtual_machine_network_interface_name
    subnet_id                     = var.network_interface_ip_configuration_subnet_id
    private_ip_address_allocation = var.network_interface_ip_configuration_private_ip_address_allocation
  }
}

######################################################
##              Windows Virtual Machine             ##
######################################################
resource "azurerm_windows_virtual_machine" "windows_virtual_machine" {
  name                  = var.windows_virtual_machine_name
  resource_group_name   = var.resource_group_name
  location              = var.location
  size                  = var.windows_virtual_machine_size
  admin_username        = var.admin_username
  admin_password        = var.admin_password
  network_interface_ids = [azurerm_network_interface.windows_virtual_machine_network_interface.id]
  computer_name         = var.computer_name
  tags                  = var.tags

  source_image_reference {
    publisher = var.windows_virtual_machine_source_image_reference_publisher
    offer     = var.windows_virtual_machine_source_image_reference_offer
    sku       = var.windows_virtual_machine_source_image_reference_sku
    version   = var.source_image_reference_version
  }

  os_disk {
    storage_account_type = var.os_disk_storage_account_type
    caching              = var.os_disk_caching
    name                 = var.windows_virtual_machine_os_disk_name
  }
}
