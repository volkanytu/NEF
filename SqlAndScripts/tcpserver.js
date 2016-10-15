var net = require('net');
var server = net.createServer(function(c) { //'connection' listener
  console.log('client connected');
  var socket = require('socket.io-client')('http://172.19.1.66:5555');
  socket.emit("push", "postfeeds");
  
c.on('end', function() {
    console.log('client disconnected');
  });
  //c.write('hello\r\n');
  c.pipe(c);
});
server.listen(8000, function() { //'listening' listener
  console.log('server bound');
});