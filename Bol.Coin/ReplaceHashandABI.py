import os
from shutil import copyfile
import json

#Copy Bol.Coin/bin/Debug/Bol.Coin.avm to Bol.Api/Bol.Coin.avm
if os.path.exists("bin\\Debug\\Bol.Coin.avm"):
  copyfile("bin\\Debug\\Bol.Coin.avm", "..\\Bol.Api\Bol.Coin.avm")
  print("Copied Bol.Coin.avm")
else:
  print("The file does not exist")
 
#Obtain new contract's hash
with open('bin\\Debug\\Bol.Coin.abi.json') as json_file:
  data = json.load(json_file)
  thehash = data['hash'][2:]
  print(thehash)

#Replace hash in protocol.json
with open('..\\Bol.Api\\protocol.json', 'r', encoding = 'utf-8-sig' ) as protocol:
  data = json.load(protocol)
  if data['ProtocolConfiguration']['BolContract']['ScriptHash'] == thehash :
    print ("The hash has not changed")
  else:
    data['ProtocolConfiguration']['BolContract']['ScriptHash'] = thehash
    protocol.close()
    print("Replaced hash in protocol.json")
    
with open('..\\Bol.Api\\protocol.json', 'w', encoding = 'utf-8-sig') as protocol:
    json.dump(data, protocol, indent = 2, sort_keys = False)
    protocol.close()
  
#Replace hash in protocol.internal.json
with open('..\\Bol.Api\\protocol.internal.json', 'r', encoding = 'utf-8-sig') as protocolInternal:
  data = json.load(protocolInternal)
  if data['ProtocolConfiguration']['BolContract']['ScriptHash'] == thehash :
    print ("The hash has not changed")
  else:
    data['ProtocolConfiguration']['BolContract']['ScriptHash'] = thehash
    protocolInternal.close()
    print("Replaced hash in protocol.internal.json")
    
with open('..\\Bol.Api\\protocol.internal.json', 'w', encoding = 'utf-8-sig') as protocolInternal:
    json.dump(data, protocolInternal, indent = 2, sort_keys = False)
    protocolInternal.close()
    
#Replace hash in protocol.mainnet.json
with open('..\\Bol.Api\\protocol.mainnet.json', 'r', encoding = 'utf-8-sig') as protocolMainnet:
  data = json.load(protocolMainnet)
  if data['ProtocolConfiguration']['BolContract']['ScriptHash'] == thehash :
    print ("The hash has not changed")
  else:
    data['ProtocolConfiguration']['BolContract']['ScriptHash'] = thehash
    protocolMainnet.close()
    print("Replaced hash in protocol.mainnet.json")
    
with open('..\\Bol.Api\\protocol.mainnet.json', 'w', encoding = 'utf-8-sig') as protocolMainnet:
    json.dump(data, protocolMainnet, indent = 2, sort_keys = False)
    protocolMainnet.close()