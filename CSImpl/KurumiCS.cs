using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

using KurumiTokenList = System.Collections.Generic.List<KurumiToken>;

using TokenType = KurumiTokenType;
using OperatorType = KurumiOperatorType;

public enum KurumiTokenType {
	Number, String, Identifier, Operator
};

public enum KurumiOperatorType {
	ASSIGN,
	COLON,
	ADD, SUB, MUL, DIV, 
	ADDASSIGN,
	SUBASSIGN,
	MULASSIGN,
	DIVASSIGN,
	EQUAL, NOTEQ, GTHAN, LTHAN, GOREQ, LOREQ, 
	OPPOSITE, 
	COMMA, 
	LBRACKET, RBRACKET, 
	ENDLINE,
	UNKNOWN
};

public enum KurumiKeyword {
	IF, ELSE, ELIF, GOTO, END, UNKNOWN
}

public enum KurumiCommandType {
	JMPIN, JMP, ASSIGN, ADD, SUB, MUL, DIV, EQUAL, NOTEQ, GTHAN, LTHAN, GOREQ, LOREQ, OPPOSITE, CALL, REF, UNKNOWN
}

public enum KurumiValueTypes : int {Null, Number, Integer, String, Function, Command, Reference};

public delegate void FuncReference(KurumiScript script, KurumiValueList args);

public abstract class KurumiValue {
	public KurumiValueTypes type;

	public KurumiValue(KurumiValueTypes type) {
		this.type = type;
	}

	public virtual KurumiValue
	dereference() {
		return this;
	}

	public virtual KurumiValue 
	add(KurumiValue other) {
		return this;
	}
	public virtual KurumiValue 
	sub(KurumiValue other) {
		return this;
	}
	public virtual KurumiValue 
	mul(KurumiValue other) {
		return this;
	}
	public virtual KurumiValue 
	div(KurumiValue other) {
		return this;
	}

	public virtual KurumiValue 
	equal(KurumiValue other) {
		return this;
	}
	public virtual KurumiValue 
	noteq(KurumiValue other) {
		return this;
	}
	public virtual KurumiValue 
	gthan(KurumiValue other) {
		return this;
	}
	public virtual KurumiValue 
	lthan(KurumiValue other) {
		return this;
	}
	public virtual KurumiValue 
	loreq(KurumiValue other) {
		return this;
	}
	public virtual KurumiValue 
	goreq(KurumiValue other) {
		return this;
	}

	public virtual KurumiNumber
	equal_number(KurumiNumber value) {
		throw new Exception("The value can not be compared with this type: " + type);
	}
	public virtual KurumiNumber
	gthan_number(KurumiNumber value) {
		throw new Exception("The value can not be compared with this type: " + type);
	}
	public virtual KurumiNumber
	lthan_number(KurumiNumber value) {
		throw new Exception("The value can not be compared with this type: " + type);
	}
	public virtual KurumiNumber
	equal_string(KurumiString value) {
		throw new Exception("The value can not be compared with this type: " + type);
	}
	public virtual KurumiNumber
	gthan_string(KurumiString value) {
		throw new Exception("The value can not be compared with this type: " + type);
	}
	public virtual KurumiNumber
	lthan_string(KurumiString value) {
		throw new Exception("The value can not be compared with this type: " + type);
	}

	public virtual KurumiValue
	negation() {
		throw new Exception("The value can not be negation with this type: " + type);
	}

	public virtual bool
	to_bool() {
		throw new Exception("Value cannot be cast to bool: " + type);
	}

	public virtual KurumiValue
	clone() {
		return this;
	}

	public virtual string 
	to_string() {
		throw new Exception("The value cannot be convert to String");
	}

	public virtual double
	to_number() {
		throw new Exception("The value cannot be convert to Number");
	}

	public virtual KurumiOperatorType
	get_operator_type() {
		throw new Exception("The value cannot be convert to OperatorType");
	}

	public virtual void
	call(KurumiScript script, KurumiValueList args) {
		throw new Exception("Type cannot be used as a function.");
	}
}

public class KurumiCommand 
: KurumiValue {
	public KurumiCommandType command_type;
	public KurumiCommand(KurumiCommandType command_type) :
	  	base(KurumiValueTypes.Command)
	{
		this.command_type = command_type;
	}

	public override string 
	to_string() {
		return command_type.ToString();
	}

	public override KurumiOperatorType
	get_operator_type() {
		return KurumiOperatorType.UNKNOWN;
	}
}

public class KurumiNull
: KurumiValue {
	public KurumiNull() :
	  	base(KurumiValueTypes.Null)
	{
	}
	public override string 
	to_string() {
		return "null";
	}

	public override bool
	to_bool() {
		return false;
	}
}

public class KurumiFunction
: KurumiValue {
	FuncReference reference;
	public KurumiFunction(FuncReference reference) :
	  	base(KurumiValueTypes.Function)
	{
		this.reference = reference;
	}

	public override void
	call(KurumiScript script, KurumiValueList args) {
		reference(script, args);
	}

	public override string
	to_string() {
		return "function";
	}

	public override bool
	to_bool() {
		return true;
	}
}

public class KurumiNumber
: KurumiValue {
	public double number;
	public KurumiNumber(string data) :
	  	base(KurumiValueTypes.Number)
	{
		try {
           	number = Convert.ToDouble(data, System.Globalization.CultureInfo.InvariantCulture);
        }
        catch( FormatException ) {
        	Console.WriteLine("'{0}' is not in a valid format.", data);
        }
	}

	public KurumiNumber(double data) :
	  	base(KurumiValueTypes.Number)
	{
		number = data;
	}
	public KurumiNumber(bool data) :
	  	base(KurumiValueTypes.Number)
	{
		number = data ? 1 : 0;
	}

	public override KurumiValue 
	add(KurumiValue other) {
		if( other.type == KurumiValueTypes.String ) {
			return other.add(this);
		}
		if( other.type == KurumiValueTypes.Number || other.type == KurumiValueTypes.Integer ) {
			return new KurumiNumber(number + ((KurumiNumber)other).number);
		}
		return this;
	}
	public override KurumiValue 
	sub(KurumiValue other) {
		if( other.type == KurumiValueTypes.Number || other.type == KurumiValueTypes.Integer ) {
			return new KurumiNumber(number - ((KurumiNumber)other).number);
		}
		return this;
	}
	public override KurumiValue 
	mul(KurumiValue other) {
		if( other.type == KurumiValueTypes.Number || other.type == KurumiValueTypes.Integer ) {
			return new KurumiNumber(number * ((KurumiNumber)other).number);
		}
		return this;
	}
	public override KurumiValue 
	div(KurumiValue other) {
		if( other.type == KurumiValueTypes.Number || other.type == KurumiValueTypes.Integer ) {
			return new KurumiNumber(number / ((KurumiNumber)other).number);
		}
		return this;
	}

	public override KurumiValue
	negation() {
		number = number > 0 ? 0 : 1;
		return this;
	}

	public override KurumiNumber
	equal_number(KurumiNumber value) {
		return new KurumiNumber(value.number== number);
	}
	public override KurumiNumber
	gthan_number(KurumiNumber value) {
		return new KurumiNumber(value.number > number);
	}
	public override KurumiNumber
	lthan_number(KurumiNumber value) {
		return new KurumiNumber(value.number < number);
	}

	public override KurumiValue 
	equal(KurumiValue other) {
		return other.equal_number(this);
	}
	public override KurumiValue 
	noteq(KurumiValue other) {
		return other.equal_number(this).negation();
	}
	public override KurumiValue 
	gthan(KurumiValue other) {
		return other.gthan_number(this);
	}
	public override KurumiValue 
	lthan(KurumiValue other) {
		return other.lthan_number(this);
	}
	public override KurumiValue 
	loreq(KurumiValue other) {
		return other.gthan_number(this).negation();
	}
	public override KurumiValue 
	goreq(KurumiValue other) {
		return other.lthan_number(this).negation();
	}

	public override KurumiValue
	clone() {
		return new KurumiNumber(number);
	}

	public override bool
	to_bool() {
		return number != 0;
	}

	public override string 
	to_string() {
		return number.ToString();
	}

	public override double
	to_number() {
		return number;
	}
}

public class KurumiReference
: KurumiValue {
	public KurumiValue reference = new KurumiNull();
	public int storage_index;

	public KurumiReference(int storage_index) :
	  	base(KurumiValueTypes.Reference)
	{
		this.storage_index = storage_index;
	}

	public override KurumiValue
	dereference() {
		return reference.clone();
	}

	public override bool
	to_bool() {
		return reference.to_bool();
	}

	public override string 
	to_string() {
		return reference.to_string();
	}

	public override void
	call(KurumiScript script, KurumiValueList args) {
		reference.call(script, args);
	}

	public override KurumiValue
	clone() {
		return reference.clone();
	}
}

public class KurumiString 
: KurumiValue {
	public StringBuilder s;
	public KurumiString(string data) :
	  	base(KurumiValueTypes.String)
	{
		s = new StringBuilder(data);
	}

	public override KurumiValue 
	equal(KurumiValue other) {
		return other.equal_string(this);
	}
	public override KurumiValue 
	noteq(KurumiValue other) {
		return other.equal_string(this).negation();
	}

	public override KurumiNumber
	equal_string(KurumiString value) {
		return new KurumiNumber(value.s.ToString() == s.ToString());
	}

	public override KurumiValue 
	add(KurumiValue other) {
		return new KurumiString(to_string() + other.to_string());
	}

	public override bool
	to_bool() {
		return s.Length != 0;
	}

	public override string 
	to_string() {
		return s.ToString();
	}

	public override double
	to_number() {
		return 0;
	}
}

public class KurumiValueList {
	public List<KurumiValue> args = new List<KurumiValue>();
	public void 
	add(KurumiValue value) {
		if( value != null ) {
			args.Add(value);
		}
	}

	public void 
	add(KurumiToken token, KurumiStorage storage) {
		switch( token.type ) {
			case TokenType.String:
				add(new KurumiString(token.data));
				break;
			case TokenType.Number:
				add(new KurumiNumber(token.data));
				break;
			case TokenType.Identifier:
				add(new KurumiCommand(KurumiCommandType.REF));
				add(new KurumiNumber(storage.get_index_from_name(token.data)));
				break;
			case TokenType.Operator:
				add(new KurumiCommand(token.optype_to_command()));
				break;
			default: break;
		}
	}

	public void
	dublicate_top(int size = 1) {
		int e = args.Count;
		int p = e - size;
		while( p < e ) {
			add(args[p++]);
		}
	}

	public KurumiValue
	pop() {
		int size = args.Count;
		if( size <= 0 ) return null;
		size -= 1;
		KurumiValue v = args[size];
		args.RemoveAt(size);
		return v;
	}

	public KurumiValue
	top() {
		int size = args.Count;
		if( size <= 0 ) return null;
		return args[size - 1];
	}

	public KurumiValue
	this[int index] {
		get {
			if( index >= args.Count ) return null;
			return args[index];
		}
	}

	public void
	debug() {
		for( int i = 0; i < args.Count; i++ ) {
			KurumiValue v = args[i];
			Console.WriteLine("Position in list: {2, 3} Value: {0, 8} Type: {1, 5}", v.to_string(), v.type, i);
		}
	}

	public int
	size() {
		return args.Count;
	}
}

public class KurumiToken {
	public string data;
	public KurumiTokenType type;
	public int indent;
	public int line;
	public int line_position;

	public KurumiToken(IterableString s, KurumiTokenType type) {
		this.data = s.substring();
		this.type = type;
		this.indent = s.get_indent();
		this.line = s.line;
		this.line_position = s.line_position - this.data.Length;
	}

	public KurumiKeyword
	is_keyword() {
		if( type == TokenType.Identifier ) {
			switch( data ) 
			{
				case "if"  : return KurumiKeyword.IF;
				case "elif": return KurumiKeyword.ELIF;
				case "else": return KurumiKeyword.ELSE;
				case "goto": return KurumiKeyword.GOTO;
				case "end" : return KurumiKeyword.END;
			}
		}
		return KurumiKeyword.UNKNOWN;
	}

	public string
	get_token_info() {
		return String.Format("Line: {0} Line Position: {1} Data: {2} Type: {3}", line, line_position, data, type);
	}

	public KurumiOperatorType
	to_operator_type() {
		if( type == TokenType.Operator ) {
			switch( data ) {
				case "," : return KurumiOperatorType.COMMA;
				case ":" : return KurumiOperatorType.COLON;
				case "+" : return KurumiOperatorType.ADD;
				case "-" : return KurumiOperatorType.SUB;
				case "*" : return KurumiOperatorType.MUL;
				case "/" : return KurumiOperatorType.DIV;
				case "=" : return KurumiOperatorType.ASSIGN;
				case "!" : return KurumiOperatorType.OPPOSITE;

				case "(" : return KurumiOperatorType.LBRACKET;
				case ")" : return KurumiOperatorType.RBRACKET;

				case "\n": return KurumiOperatorType.ENDLINE;

				case ">" : return KurumiOperatorType.GTHAN;
				case "<" : return KurumiOperatorType.LTHAN;
				case ">=": return KurumiOperatorType.GOREQ;
				case "<=": return KurumiOperatorType.LOREQ;

				case "==": return KurumiOperatorType.EQUAL;
				case "!=": return KurumiOperatorType.NOTEQ;

				case "+=": return KurumiOperatorType.ADDASSIGN;
				case "-=": return KurumiOperatorType.SUBASSIGN;
				case "*=": return KurumiOperatorType.MULASSIGN;
				case "/=": return KurumiOperatorType.DIVASSIGN;
			}
		}
		return KurumiOperatorType.UNKNOWN;
	}

	public KurumiCommandType
	optype_to_command() {
		switch( to_operator_type() ) {
			case KurumiOperatorType.ADD: return KurumiCommandType.ADD;
			case KurumiOperatorType.SUB: return KurumiCommandType.SUB;
			case KurumiOperatorType.MUL: return KurumiCommandType.MUL;
			case KurumiOperatorType.DIV: return KurumiCommandType.DIV;

			case KurumiOperatorType.ADDASSIGN: return KurumiCommandType.ADD;
			case KurumiOperatorType.SUBASSIGN: return KurumiCommandType.SUB;
			case KurumiOperatorType.MULASSIGN: return KurumiCommandType.MUL;
			case KurumiOperatorType.DIVASSIGN: return KurumiCommandType.DIV;

			case KurumiOperatorType.GTHAN: return KurumiCommandType.GTHAN;
			case KurumiOperatorType.LTHAN: return KurumiCommandType.LTHAN;
			case KurumiOperatorType.GOREQ: return KurumiCommandType.GOREQ;
			case KurumiOperatorType.LOREQ: return KurumiCommandType.LOREQ;
			case KurumiOperatorType.EQUAL: return KurumiCommandType.EQUAL;
			case KurumiOperatorType.NOTEQ: return KurumiCommandType.NOTEQ;
			case KurumiOperatorType.OPPOSITE: return KurumiCommandType.OPPOSITE;
			case KurumiOperatorType.ASSIGN: return KurumiCommandType.ASSIGN;
			default: return KurumiCommandType.UNKNOWN;
		}
	}
}

public class IterableString {
	public string s;
	public int position = 0;
	public int prev_position = 0;

	public int line = 1;
	public int line_position = 1;

	public IterableString(string s) {
		this.s = s;
	}

	public char 
	current {
		get {
			return s[position];
		}
	}

	public bool
	is_end() {
		return position >= s.Length;
	}

	public int
	skip_empty_lines() {
		int lines = 0;
		for( int i = position; i < s.Length; i++ ) {
			if( s[i] != ' ' && s[i] != '\t' && s[i] != '\r' && s[i] != '\n' ) {
				break;
			}
			if( s[i] == '\n' ) {
				lines += 1;
				position = i + 1;
			}
		}
		line += lines;
		return lines;
	}

	public void
	next() {
		if( current == '\t' ) {
			line_position += 4;
		} else {
			line_position += 1;
		}
		position += 1;
	}

	public void
	prev() {
		position -= 1;
	}

	public void
	store_position() {
		prev_position = position;
	}

	public string
	substring() {
		return s.Substring(prev_position, position - prev_position);
	}

	public bool
	is_digit() {
		return Char.IsDigit(current);
	}
	public bool
	is_letter_or_digit() {
		return Char.IsLetterOrDigit(current);
	}
	public bool
	is_name_identifier() {
		char c = current;
		return Char.IsLetter(c) || c == '_';
	}
	public bool
	is_string() {
		char c = current;
		return c == '"';
	}
	public bool
	is_operator(char c) {
		return 
			c == '+' || c == '-' || 
			c == '/' || c == '*' || 
			c == '>' || c == '<' || 
			c == '=' || c == '!' || 
			c == '(' || c == ')' ||
			c == ',' || c == ':';
	}
	public bool
	is_valid_operator(string s) {
		if( s.Length < 2 ) return is_operator(s[0]);
		return 
			s == ">=" || s == "<=" || 
			s == "!=" || s == "==" || 
			s == "+=" || s == "-=" ||
			s == "*=" || s == "/=";
	}

	public bool
	is_endline() {
		return current == '\n';
	}
	public bool
	is_tab() {
		return current == '\t';
	}
	public bool
	is_space() {
		return current == ' ';
	}

	public int
	get_indent() {
		int indent = 0;
		int i = prev_position - 1;
		if( i > 0 ) {
			if( s[i] == '"' ) i--;
			for( ; i >= 0; i-- ) {
				if( s[i] == ' ' ) {
					indent += 1;
				} else if( s[i] == '\t' ) {
					indent += 2;
				} else {
					break;
				}
			}
		}
		return indent;
	}

	public KurumiTokenList 
	tokenize() {
		KurumiTokenList list = new KurumiTokenList();
		while( !is_end() ) {
			KurumiToken token = parse_any();
			if( token != null ) {
				list.Add(token);
				//Console.WriteLine("{0} {1} {2} {3} {4}", 
				//token.data, token.type, token.line, token.line_position, token.indent);
			}
		}

		return list;
	}

	public KurumiToken
	parse_any() {
		if( is_string() ) {
			return parse_string();
		} if( is_digit() ) {
			return parse_number();
		} if( is_name_identifier() ) {
			return parse_name();
		} if( is_operator(current) ) {
			return parse_operator();
		} if( is_endline() ) {
			store_position();
			next();
			KurumiToken token = new KurumiToken(this, TokenType.Operator);
			line_position = 1;
			line += 1;
			skip_empty_lines();
			return token;
		} else {
			next();
		}
		return null;
	}

	public KurumiToken
	parse_string() {
		next();
		store_position();
		for( ; !is_end(); next() ) {
			if( is_string() ) {
				break;
			}
		}
		KurumiToken token = new KurumiToken(this, TokenType.String);
		next();
		return token;
	}

	public KurumiToken
	parse_number() {
		store_position();
		for( ; !is_end(); next() ) {
			if( !is_digit() && current != '.' ) {
				break;
			}
		}
		KurumiToken token = new KurumiToken(this, TokenType.Number);
		return token;
	}

	public KurumiToken
	parse_name() {
		store_position();
		for( ; !is_end(); next() ) {
			if( !is_letter_or_digit() && current != '_' ) {
				break;
			}
		}
		KurumiToken token = new KurumiToken(this, TokenType.Identifier);
		return token;
	}

	public KurumiToken
	parse_operator() {
		store_position();
		for( int i = 0; !is_end(); next() ) {
			i++;
			if( !is_operator(current) || i > 2 ) {
				if( !is_valid_operator(substring()) ) {
					prev();
				}
				break;
			}
		}
		KurumiToken token = new KurumiToken(this, TokenType.Operator);
		return token;
	}
}

public static class KurumiLibrary {
	public static void 
	print(KurumiScript script, KurumiValueList args) {
		int size = args.size();
		for( int i = 0; i < size; i++ ) {
			KurumiValue v = args.pop();
			Console.WriteLine(v.to_string());
		}
		args.add(new KurumiNull());
	}

	public static void
	floor(KurumiScript script, KurumiValueList args) {
		KurumiNumber value = (KurumiNumber)args.pop();
		value.number = Math.Floor(value.number);
		args.add(value);
	}
}

public class KurumiScript {
	KurumiStorage storage;
	static int max_stack_size = 128;

	KurumiValue[] value_stack = new KurumiValue[max_stack_size];
	int sp = 0;
	int vp = 0;
	
	public bool freeze = false;

	KurumiValueList expression;
	KurumiReference[] data;

	public KurumiScript(KurumiStorage storage, KurumiValueList expression) {
		int storage_size = storage.size();
		this.storage = storage;
		this.expression = expression;
		data = new KurumiReference[storage_size];
		for( int i = 0; i < storage_size; i++ ) {
			data[i] = new KurumiReference(i);
		}
		init_library();
	}

	public void
	init_library() {
		set_function("print", KurumiLibrary.print);
		set_function("floor", KurumiLibrary.floor);
	}

	void 
	register_global_value(int data_index, KurumiValue value) {
		if( data_index < data.Length ) {
			data[data_index].reference = value;
		}
	}

	public void
	set_value(string name, KurumiValue value) {
		register_global_value(storage.get_index_from_name(name), value);
	}

	public KurumiValue
	get_value(int data_index) {
		return data[data_index].reference;
	}

	public void
	set_function(string name, FuncReference reference) {
		register_global_value(storage.get_index_from_name(name), new KurumiFunction(reference));
	}

	public void
	interpret() {
		while( vp < expression.size() && !freeze ) {
			KurumiValue value = expression[vp++];
			switch( value.type ) {
				case KurumiValueTypes.Command:
					execute_command((KurumiCommand)value);
					break;
				case KurumiValueTypes.String:
				case KurumiValueTypes.Number:
					value_stack[sp++] = value;
					break;
			}
		}
	}

	public void
	stack_debug() {
		for( int i = 0; i < sp; i++ ) {
			KurumiValue v = value_stack[i];
			Console.WriteLine("{0}", v.to_string());
		}
	}

	//JMPIN, JMP, ASSIGN, ADD, SUB, MUL, DIV, EQUAL, NOTEQ, GTHAN, LTHAN, GOREQ, LOREQ, OPPOSITE, CALL, REF
	public void
	execute_command(KurumiCommand command) {
		//Console.WriteLine("execute_command: {0}", command.command_type.ToString());
		//stack_debug();
		switch( command.command_type ) {
			case KurumiCommandType.JMPIN: {
				KurumiValue jmpp = expression[vp++];
				KurumiValue expv = value_stack[--sp];
				if( expv.to_bool() == false ) {
					vp = (int)jmpp.to_number();
				}
				break;
			}
			case KurumiCommandType.JMP: {
				KurumiValue jmpp = expression[vp++];
				vp = (int)jmpp.to_number();
				break;
			}
			case KurumiCommandType.ASSIGN: {
				KurumiValue rexp = value_stack[--sp];
				KurumiValue lexp = value_stack[--sp];
				((KurumiReference)lexp).reference = rexp.clone();
				break;
			}
			case KurumiCommandType.ADD: {
				KurumiValue rexp = value_stack[--sp].dereference();
				KurumiValue lexp = value_stack[--sp].dereference();
				value_stack[sp++] = rexp.add(lexp);
				break;
			}
			case KurumiCommandType.SUB: {
				KurumiValue rexp = value_stack[--sp].dereference();
				KurumiValue lexp = value_stack[--sp].dereference();
				value_stack[sp] = lexp.sub(rexp);
				sp++;
				break;
			}
			case KurumiCommandType.MUL: {
				KurumiValue rexp = value_stack[--sp].dereference();
				KurumiValue lexp = value_stack[--sp].dereference();
				value_stack[sp++] = lexp.mul(rexp);
				break;
			}
			case KurumiCommandType.DIV: {
				KurumiValue rexp = value_stack[--sp].dereference();
				KurumiValue lexp = value_stack[--sp].dereference();
				value_stack[sp++] = lexp.div(rexp);
				break;
			}
			case KurumiCommandType.EQUAL: {
				KurumiValue rexp = value_stack[--sp].dereference();
				KurumiValue lexp = value_stack[--sp].dereference();
				value_stack[sp++] = lexp.equal(rexp);
				break;
			}
			case KurumiCommandType.NOTEQ: {
				KurumiValue rexp = value_stack[--sp].dereference();
				KurumiValue lexp = value_stack[--sp].dereference();
				value_stack[sp++] = lexp.noteq(rexp);
				break;
			}
			case KurumiCommandType.GTHAN: {
				KurumiValue rexp = value_stack[--sp].dereference();
				KurumiValue lexp = value_stack[--sp].dereference();
				value_stack[sp++] = lexp.gthan(rexp);
				break;
			}
			case KurumiCommandType.LTHAN: {
				KurumiValue rexp = value_stack[--sp].dereference();
				KurumiValue lexp = value_stack[--sp].dereference();
				value_stack[sp++] = lexp.lthan(rexp);
				break;
			}
			case KurumiCommandType.GOREQ: {
				KurumiValue rexp = value_stack[--sp].dereference();
				KurumiValue lexp = value_stack[--sp].dereference();
				value_stack[sp++] = lexp.goreq(rexp);
				break;
			}
			case KurumiCommandType.LOREQ: {
				KurumiValue rexp = value_stack[--sp].dereference();
				KurumiValue lexp = value_stack[--sp].dereference();
				value_stack[sp++] = lexp.loreq(rexp);
				break;
			}
			case KurumiCommandType.CALL: {
				KurumiValue func = value_stack[--sp];
				KurumiValue retc = expression[vp++];
				KurumiValue size = expression[vp++];
				int sz = (int)size.to_number();
				int rc = (int)retc.to_number();
				KurumiValueList args = new KurumiValueList();
				if( sz > 0 ) {
					for( int i = 0; i < sz; i++ ) {
						args.add(value_stack[--sp].dereference());
					}
				}
				func.call(this, args);
				if( rc > 0 ) {
					value_stack[sp++] = args.top();
				}
				break;
			}
			case KurumiCommandType.REF: {
				KurumiValue refv = expression[vp++];
				value_stack[sp++] = data[(int)refv.to_number()];
				break;
			}
		}
		//Console.WriteLine("after_execute_command");
		//stack_debug();
	}
}

public class KurumiGotoStarage {
	Dictionary<string, KurumiNumber> goto_pos = new Dictionary<string, KurumiNumber>();
	public KurumiNumber
	get(string name) {
		if( goto_pos.ContainsKey(name) ) {
			return goto_pos[name];
		}
		KurumiNumber v = new KurumiNumber(-1);
		goto_pos[name] = v;
		return v;
	}
}

public class KurumiStorage {
	int storage_index = 0;
	Dictionary<string, int> reference_indices = new Dictionary<string, int>();
	public int
	size() {
		return reference_indices.Count;
	}

	public int
	get_index_from_name(string name) {
		if( reference_indices.ContainsKey(name) ) {
			return reference_indices[name];
		} else {
			int index = storage_index++;
			reference_indices[name] = index;
			return index;
		}
	}
}

public class KurumiParser {
	int value_i = 0;
	int if_statement_count = 0;

	KurumiTokenList list;
	KurumiValueList expression;
	KurumiGotoStarage goto_storage;
	KurumiStorage storage;

	public KurumiParser() {
	}

	public KurumiScript
	parse(string string_data) {
		try {
			expression = new KurumiValueList();
			goto_storage = new KurumiGotoStarage();
			value_i = 0;
			if_statement_count = 0;
			storage = new KurumiStorage();
			IterableString s = new IterableString(string_data);
			list = s.tokenize();
			generate_code();

			return new KurumiScript(storage, expression);
		} catch( Exception e ) {
			Console.WriteLine("{0}", e.Message);
			return null;
		}
	}

	KurumiToken
	get_current_token(int offset = 0) {
		if( value_i+offset >= list.Count ) return null;
		return list[value_i+offset];
	}

	void
	next() {
		value_i += 1;
	}

	public void
	generate_code() {
		for( ; value_i < list.Count; ) {
			lexpr();
		}
		expression.debug();
	}

	public void
	lexpr() {
		KurumiToken current = get_current_token();
		switch( current.type ) {
			case TokenType.Identifier: 
				KurumiKeyword keyword = current.is_keyword();
				if( keyword == KurumiKeyword.UNKNOWN ) {
					KurumiToken n = get_current_token(1);
					if( n == null ) return;
					OperatorType type = n.to_operator_type();
					if( type == OperatorType.COLON ) {
						KurumiNumber number = goto_storage.get(current.data);
						number.number = expression.size();
						next();
						next();
					} else if( type == OperatorType.LBRACKET ) {
						function_call(current, 0);
					} else {
						rexpr();
					}
				} else {
					keyword_expression(keyword);
				}
				break;
			case TokenType.Operator:
				next();
				return;
			default:
				throw new Exception("Identifier or Keyword is expected in the left-sided expression.");
		}
	}

	public void
	rexpr() {
		KurumiToken current = get_current_token();
		while( current != null ) {
			OperatorType type = current.to_operator_type();
			bool v1 = current.type == TokenType.Operator && (type == OperatorType.ENDLINE || type == OperatorType.COLON);
			bool v2 = current.is_keyword() == KurumiKeyword.END;
			if( v1 || v2 ) {
				break;
			}
			int assigment_count = 0;
			assign_expr(ref assigment_count);
			current = get_current_token();
		}
	}

	public void
	keyword_expression(KurumiKeyword keyword) {
		switch( keyword ) {
			case KurumiKeyword.IF:
				KurumiNumber stop_p = new KurumiNumber(-1);
				if_statement(stop_p);
				break;
			case KurumiKeyword.ELSE:
			case KurumiKeyword.ELIF:
				if( if_statement_count <= 0 ) {
					throw new Exception("Keyword 'else' or 'elif' without if statement.");
				}
				break;
			case KurumiKeyword.GOTO:
				next();
				KurumiToken current = get_current_token();
				expression.add(new KurumiCommand(KurumiCommandType.JMP));
				expression.add(goto_storage.get(current.data));
				next();
				break;
			default: break;
		}
	}

	public void
	if_statement(KurumiNumber stop_p) {
		next();
		if_statement_count += 1;
		rexpr();
		next();

		KurumiNumber next_p = new KurumiNumber(-1);
		expression.add(new KurumiCommand(KurumiCommandType.JMPIN));
		expression.add(next_p);

		bool additional_branching = false;

		KurumiToken current = get_current_token();
		while( current != null ) {
			KurumiKeyword kw = current.is_keyword();
			if( kw == KurumiKeyword.END ) {
				int end_position = expression.size();
				stop_p.number = end_position;
				if( !additional_branching ) {
					next_p.number = end_position;
				}
				break;
			} else if( kw == KurumiKeyword.ELSE ) {
				additional_branching = true;
				expression.add(new KurumiCommand(KurumiCommandType.JMP));
				expression.add(stop_p);
				next_p.number = expression.size();
				next();
			} else if( kw == KurumiKeyword.ELIF ) {
				additional_branching = true;
				expression.add(new KurumiCommand(KurumiCommandType.JMP));
				expression.add(stop_p);
				next_p.number = expression.size();
				if_statement(stop_p);
				break;
			} else {
				lexpr();
			}
			current = get_current_token();
		}

		next();
		if_statement_count -= 1;
	}

	void
	assign_expr(ref int assigment_count) {
		equals_expr();
		KurumiToken op = get_current_token();

		bool loop_flag = true;
		while( op != null && loop_flag ) {
			switch( op.to_operator_type() ) 
			{
				case KurumiOperatorType.ASSIGN:
					if( ++assigment_count > 1 ) {
						expression.dublicate_top();
					}
					next();
					assign_expr(ref assigment_count);
					expression.add(op, storage);
					break;
				case KurumiOperatorType.ADDASSIGN:
				case KurumiOperatorType.SUBASSIGN:
				case KurumiOperatorType.MULASSIGN:
				case KurumiOperatorType.DIVASSIGN:
					if( ++assigment_count > 1 ) {
						expression.dublicate_top();
					}
					next();
					expression.dublicate_top(2);

					assign_expr(ref assigment_count);
					expression.add(op, storage);
					expression.add(new KurumiCommand(KurumiCommandType.ASSIGN));
					break;
				default:
					loop_flag = false; 
					break;
			}
			op = get_current_token();
		}
	}

	void
	equals_expr() {
		addsub_expr();
		KurumiToken op = get_current_token();

		bool loop_flag = true;
		while( op != null && loop_flag ) {
			switch( op.to_operator_type() ) 
			{
				case KurumiOperatorType.LTHAN:
				case KurumiOperatorType.GTHAN:
				case KurumiOperatorType.LOREQ:
				case KurumiOperatorType.GOREQ:
				case KurumiOperatorType.NOTEQ:
				case KurumiOperatorType.EQUAL:
					next(); 
					addsub_expr();
					expression.add(op, storage);
					break;
				default:
					loop_flag = false; 
					break;
			}
			op = get_current_token();
		}
	}

	void
	addsub_expr() {
		muldiv_expr();
		KurumiToken op = get_current_token();

		bool loop_flag = true;
		while( op != null && loop_flag ) {
			switch( op.to_operator_type() ) 
			{
				case KurumiOperatorType.ADD:
				case KurumiOperatorType.SUB:
					next(); 
					muldiv_expr();
					expression.add(op, storage);
					break;
				default:
					loop_flag = false; 
					break;
			}
			op = get_current_token();
		}
	}

	void
	muldiv_expr() {
		variable();
		KurumiToken op = get_current_token();

		bool loop_flag = true;
		while( op != null && loop_flag ) {
			switch( op.to_operator_type() ) 
			{
				case KurumiOperatorType.MUL:
				case KurumiOperatorType.DIV:
					next(); 
					variable();
					expression.add(op, storage);
					break;
				default:
					loop_flag = false; 
					break;
			}
			op = get_current_token();
		}
	}

	void
	variable() {
		KurumiToken token = get_current_token();
		switch( token.type ) {
			case TokenType.Operator: {
				switch( token.to_operator_type() ) {
					case OperatorType.LBRACKET:
						next();
						equals_expr();
						KurumiToken rbracket = get_current_token();
						if( rbracket == null || 
							rbracket.type != TokenType.Operator || 
							rbracket.to_operator_type() != OperatorType.RBRACKET ) {
							throw new Exception("Expression right bracket not found: " + token.get_token_info());
						}
						next();
						break;
					case OperatorType.ADD:
						next();
						break;
					case OperatorType.SUB:
						next();
						expression.add(new KurumiNumber(0));
						variable();
						expression.add(token, storage);
						break;
					case OperatorType.ASSIGN:
						next();
						rexpr();
						expression.add(token, storage);
						break;
					default: 
						throw new Exception("Expression error: " + token.get_token_info());
				}
				break;
			}
			case TokenType.Identifier:
				next();
				KurumiToken n = get_current_token();
				if( n.to_operator_type() == OperatorType.LBRACKET ) {
					function_call(token, 1);
				} else {
					expression.add(token, storage);
				}
				break;

			case TokenType.Number:
			case TokenType.String:
				next();
				expression.add(token, storage);
				break;
			default: 
				throw new Exception("Expression end in unknown token.");
		}
	}

	public void
	function_call(KurumiToken token, int return_count) {
		int count = call_args();
		expression.add(token, storage);
		expression.add(new KurumiCommand(KurumiCommandType.CALL));
		expression.add(new KurumiNumber(return_count));
		expression.add(new KurumiNumber(count));
	}

	public int
	call_args() {
		int count = 0;
		next();
		KurumiToken token = get_current_token();
		OperatorType t = token.to_operator_type();
		while( token != null && t != OperatorType.RBRACKET && t != OperatorType.ENDLINE ) {
			count += 1;
			equals_expr();
			token = get_current_token();
			t = token.to_operator_type();
			if( t == OperatorType.COMMA ) {
				next();
			}
		}
		next();
		return count;
	}

	static void Main()
    {
    	Console.WriteLine("Kurumi Lang 0.0.4 beta");
    	string text_data = System.IO.File.ReadAllText("script.txt");
    	KurumiParser parser = new KurumiParser();
    	KurumiScript script = parser.parse(text_data);
    	script.interpret();

    	Console.ReadLine();
    }
}