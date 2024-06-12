output "virtual_network_id" {
  value = azurerm_virtual_network.virtual_network.id
}

output "virtual_network_name" {
  value = azurerm_virtual_network.virtual_network.name
}

output "vm_subnet_id" {
  value = [for s in azurerm_subnet.subnet : s.id if contains(split("-", s.name), "vm")][0]
}

output "pep_subnet_id" {
  value = [for s in azurerm_subnet.subnet : s.id if contains(split("-", s.name), "pep")][0]
}

output "bas_subnet_id" {
  value = [for s in azurerm_subnet.subnet : s.id if contains(split("-", s.name), "AzureBastionSubnet")][0]
}