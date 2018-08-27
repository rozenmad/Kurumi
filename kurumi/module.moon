Util = require "kurumi.util"
Lex = require "kurumi.lex"
StringIterator = require "kurumi.string_iterator"

Interpreter = require "kurumi.interpreter"

import is_digit, is_space, is_alnum, is_alpha, is_dot, is_underscore, is_endline, is_sharp, is_quote
	from Util

import token_t, is_kw, is_op, Token
	from Lex

class ModuleParser
	new: (text) =>
		@dependencies = {} -- dep modules
		@iter = StringIterator text
		@_i_position = 1

		@token_list = {}

		state = @parse!
		@print! if state ~= false

		interpreter = Interpreter @token_list
		print interpreter\expr!

	print: =>
		for k,v in pairs @token_list
			value = if v.value ~= nil then v.value.text else ""
			print v

	parse_error: (s) =>
		line = @iter\count(is_endline, @_i_position)
		print "Parse error: '#{s.text}' in line: #{line + 1}"

	from_file: (filereader, filename) ->
		return ModuleParser filereader.open filename

	parse: =>
		c = @iter\get!

		while c
			@parse_spaces!
			@_i_position = @iter.i
			c = @iter\get!
			break if not c

			token = nil
			if is_digit(c)
				token = @parse_number!
			else if is_alpha(c) or is_underscore(c)
				token = @parse_word!
			else if is_sharp(c)
				token = @parse_line_comment!
			else if is_quote(c)
				token = @parse_string!
			else if is_endline(c)
				token = @new_token(@_i_position, token_t.Endline)
				@iter\next!
			else
				token = @parse_op!

			return false if not token

		return @new_token(@_i_position, token_t.EOF)

	new_token: (position, t, v) =>
		token = Token(position, t, v)
		table.insert @token_list, token
		return token

	parse_spaces: =>
		@iter\get_until (c) -> 
			return is_space(c)

	parse_line_comment: =>
		filter = (c, n) -> 
			return not is_endline(c)

		s = @iter\sub_until filter
		return @new_token(@_i_position, token_t.Comment, s)

	parse_number: =>
		filter = (c, n) -> 
			return is_alnum(c) or (is_dot(c) and is_digit(n))

		s = @iter\sub_until filter
		d = s\count is_dot
			
		if d > 1 or s\count(is_alpha) > 0
			@parse_error s
			return nil
		
		t = if d > 0 then token_t.Real else token_t.Integer

		return @new_token(@_i_position, t, s)

	parse_word: =>
		filter = (c, n) -> 
			return is_alnum(c) or is_underscore(c)

		s = @iter\sub_until filter
		t = if is_kw(s.text) then token_t.Keyword else token_t.Word
		return @new_token(@_i_position, t, s)

	parse_op: =>
		filter = (c, n) ->
			return not is_alnum(c) and not is_space(c) and not is_endline(c)

		full_string = @iter\sub_until filter

		s = full_string
		while #s.text > 0 and not is_op(s.text)
			s = s\sub 1, #s.text - 1
			@iter\prev!

		if #s.text <= 0
			 @parse_error full_string
			 return nil

		return @new_token(@_i_position, token_t.Operator, s)

	parse_string: =>
		q = @iter\get!
		filter = (c, n, p) ->
			return c ~= q or (c == q and p == '\\')

		@iter\next!
		s = @iter\sub_until filter
		c = @iter\get!
		if c ~= q
			s = s\sub_until (c) -> return not is_endline(c) 
			@parse_error s
			return nil
		@iter\next!

		return @new_token(@_i_position, token_t.String, s)