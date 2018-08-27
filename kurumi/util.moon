class String
	-- with skip empty lines
	linebyline: (s, pattern) ->
		t = {}
		for c in s\gmatch "[^\n]+" 
			table.insert t, c
		return t

class Util
	@string = String

	is_digit: (c) -> return c and (c >= '0' and c <= '9')
	is_space: (c) -> return c and (c == ' ' or c == '\r' or c == '\t')
	is_alpha: (c) -> return c and (c >= 'a' and c <= 'z' or c >= 'A' and c <= 'Z')
	is_sharp: (c) -> return c and (c == '#')
	is_quote: (c) -> return c and (c == '"' or c == '\'')
	is_alnum: (c) -> return Util.is_digit(c) or Util.is_alpha(c)
	is_dot: (c) -> return c and (c == '.')
	is_underscore: (c) -> return c and (c == '_')
	is_endline: (c) -> return c and (c == '\n')

	find: (list, item) ->
		for v in *list
			return true if item == v
		return false

	find_key: (t, value) ->
		for k, v in pairs t
			return k if v == value

return Util