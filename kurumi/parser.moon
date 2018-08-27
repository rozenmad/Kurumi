ModuleParser = require "kurumi.module"

class DefaultFileReader
	open: (name) ->
		file = assert io.open name, "r"
		text = file\read "*all"
		file\close!
		return text

class Parser
	new: (@filereader = DefaultFileReader) =>
		@modules = {}

	from_file: (filename) =>
		kurumi_module = ModuleParser.from_file @filereader, filename

	from_data: (filedaata) =>
		--impl
