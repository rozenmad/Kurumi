Lex = require "kurumi.lex"

import token_t from Lex

class Interpreter
	new: (@token_list) =>
		@i = 1
		@current_token = nil
		@next!

	make_error: =>
		print 'Interpet error (unrecognized token): ', @to_value!

	next: =>
		@current_token = @token_list[@i]
		@i += 1

	term: =>
		result = @factor!

		t = @to_value!
		while t == '*' or t == '/'
			value = @to_value!
			switch value
				when '*'
					@next!
					result *= @factor!
				when '/'
					@next!
					result /= @factor!
				else
					@make_error!
					return nil
			t = @to_value!

		return result

	factor: =>
		t = @current_token.t
		result = nil
		switch t
			when token_t.Integer, token_t.Real
				result = tonumber @to_value!
				@next!
			when token_t.Operator
				v = @to_value!
				if v == '('
					@next!
					result = @expr!
					@next!

		return result

	to_value: =>
		return tostring(@current_token.value)

	expr: =>
		result = @term!

		t = @to_value!
		while t == '+' or t == '-'
			value = @to_value!
			switch value
				when '+'
					@next!
					result += @term!
				when '-'
					@next!
					result -= @term!
				else
					@make_error!
					return nil
			t = @to_value!

		return result

		--print result


