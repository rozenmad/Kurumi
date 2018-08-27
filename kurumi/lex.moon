Util = require "kurumi.util" 

oplist = {
	'=', '+', '-', '*', 
	'/', '@', '.', ':', 
	'<', '>', ',', 

	'[', ']', '(', ')', 
	'{', '}', 

	'+=', '-=', 
	'/=', '*=',
	'^=', '==',
	'!=', '<=',
	'>=', '->',
	'...',
}

kwlist = {
	"class", "extends", "end", "if", "elif", "else", "constructor", "super", "return", "or", "and", "not"
}

token_t = {
	Comment: 1, Real: 2, Integer: 3, Word: 4, Keyword: 5, Operator: 6, String: 7, Endline: 8, EOF: 9
}

is_op = (s) ->
	return Util.find(oplist, s)

is_kw = (s) ->
	return Util.find(kwlist, s)

class Token
	new: (@position, @t, @value = nil) =>
	__tostring: =>
		value = tostring(@value)
		t = Util.find_key(token_t, @t) or ""

		return string.format "%6i %10s %10s", @position, t, value

return {
	:Token, :token_t, :value_t, :is_op, :is_kw
}