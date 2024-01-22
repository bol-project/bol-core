import os
from shutil import copyfile
import json
import sys

configuration = sys.argv[1]
bin_path = f"bin/{configuration}/net8.0/"
avm_path = bin_path + "Bol.Coin.avm"
abi_path = bin_path + "Bol.Coin.abi.json"

def hash_replace(path, contract_hash, json_path):
  with open(path, 'r', encoding = 'utf-8-sig' ) as protocol:
    data = json.load(protocol)
    
    if json_path(data, None) == contract_hash :
      print ("The hash has not changed")
      return
    
    json_path(data,contract_hash)
    protocol.close()
    
  with open(path, 'w', encoding = 'utf-8-sig') as protocol:
    json.dump(data, protocol, indent = 2, sort_keys = False)
    protocol.close()
  print("Replaced hash in " + path)

 
def protocol_path(data, value): 
  if value is not None:
    data['ProtocolConfiguration']['BolContract']['ScriptHash'] = value
  return data['ProtocolConfiguration']['BolContract']['ScriptHash']

 
def appsettings_path(data, value): 
  if value is not None:
    data['BolConfig']['Contract'] = value
  return data['BolConfig']['Contract']
  
#Copy Bol.Coin/bin/Debug/Bol.Coin.avm to Bol.Api/Bol.Coin.avm
if os.path.exists(avm_path):
  copyfile(avm_path, "../Bol.Api/Bol.Coin.avm")
  copyfile(avm_path, "../Bol.Coin.Tests/Bol.Coin.avm")
  print("Copied Bol.Coin.avm")
else:
  print("The file does not exist")
 
#Obtain new contract's hash
with open(abi_path) as json_file:
  data = json.load(json_file)
  the_hash = data['hash'][2:]
  print(the_hash)

hash_replace('../Bol.Api/protocol.json', the_hash, protocol_path)
hash_replace('../Bol.Api/protocol.mainnet.json', the_hash, protocol_path)
hash_replace('../Bol.Api/protocol.private.json', the_hash, protocol_path)

hash_replace('../Bol.Api/appsettings.json', the_hash, appsettings_path)
hash_replace('../Bol.Api/appsettings.Development.json', the_hash, appsettings_path)
hash_replace('../Bol.Coin.Tests/appsettings.json', the_hash, appsettings_path)
