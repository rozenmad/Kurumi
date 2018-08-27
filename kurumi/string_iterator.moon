class StringIterator
	new: (@text) =>
		@i = 1

	next: =>
		@i += 1
	prev: =>
		@i -= 1

	get: (offset = 0) =>
		pos = @i + offset
		return nil if pos <= 0 or pos > #@text
		return @text\sub pos, pos

	sub: (i, j) =>
		return StringIterator @text\sub i, j

	count: (condition, length = #@text) =>
		counter = 0
		for i = 1, length
			c = @text\sub i, i
			counter += 1 if condition(c) == true
		return counter

	get_until: (filter) =>
		c_prev = nil
		c_curr = @get 0
		c_next = @get 1
		while c_curr ~= nil and filter(c_curr, c_next, c_prev)
			@next!
			c_prev = c_curr
			c_curr = c_next
			c_next = @get 1
		return @i - 1

	sub_until: (filter) =>
		i = @i
		j = @get_until filter
		return @sub i, j

	__tostring: =>
		return @text

return StringIterator