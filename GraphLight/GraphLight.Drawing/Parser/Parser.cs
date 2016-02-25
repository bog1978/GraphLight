using System.Windows.Media;



using System;

namespace GraphLight.Parser {



public partial class MyParser
{
	public const int _EOF = 0;
	public const int _ident = 1;
	public const int _digraph = 2;
	public const int _lbrace = 3;
	public const int _rbrace = 4;
	public const int _lbrack = 5;
	public const int _rbrack = 6;
	public const int _arrow = 7;
	public const int _assgn = 8;
	public const int _string = 9;
	public const int _label = 10;
	public const int _rank = 11;
	public const int _order = 12;
	public const int _color = 13;
	public const int _category = 14;
	public const int _thickness = 15;
	public const int _weight = 16;
	public const int _number = 17;
	public const int _argb = 18;
	public const int maxT = 37;

	const bool T = true;
	const bool x = false;
	const int minErrDist = 2;
	
	public MyScanner scanner;
	public Errors  errors;

	public Token t;    // last recognized token
	public Token la;   // lookahead token
	int errDist = minErrDist;



	public MyParser(MyScanner scanner) {
		this.scanner = scanner;
		errors = new Errors();
	}

	void SynErr (int n) {
		if (errDist >= minErrDist) errors.SynErr(la.line, la.col, n);
		errDist = 0;
	}

	public void SemErr (string msg) {
		if (errDist >= minErrDist) errors.SemErr(t.line, t.col, msg);
		errDist = 0;
	}
	
	void Get () {
		for (;;) {
			t = la;
			la = scanner.Scan();
			if (la.kind <= maxT) { ++errDist; break; }

			la = t;
		}
	}
	
	void Expect (int n) {
		if (la.kind==n) Get(); else { SynErr(n); }
	}
	
	bool StartOf (int s) {
		return set[s, la.kind];
	}
	
	void ExpectWeak (int n, int follow) {
		if (la.kind == n) Get();
		else {
			SynErr(n);
			while (!StartOf(follow)) Get();
		}
	}


	bool WeakSeparator(int n, int syFol, int repFol) {
		int kind = la.kind;
		if (kind == n) {Get(); return true;}
		else if (StartOf(repFol)) {return false;}
		else {
			SynErr(n);
			while (!(set[syFol, kind] || set[repFol, kind] || set[0, kind])) {
				Get();
				kind = la.kind;
			}
			return StartOf(syFol);
		}
	}

	
	void GRAPH() {
		Expect(2);
		Expect(1);
		createGraph(t.val); 
		Expect(3);
		if (la.kind == 19) {
			NODES();
		}
		EDGES();
		Expect(4);
	}

	void NODES() {
		Expect(19);
		while (la.kind == 1) {
			Get();
			createNode(); 
			if (la.kind == 5) {
				NODE_ATTRS();
			}
			Expect(20);
		}
	}

	void EDGES() {
		Expect(21);
		while (la.kind == 1) {
			Get();
			createEdgeChain(); 
			EDGE();
			while (la.kind == 7) {
				EDGE();
			}
			if (la.kind == 5) {
				EDGE_ATTRS();
			}
			Expect(20);
		}
	}

	void NODE_ATTRS() {
		Expect(5);
		if (la.kind == 10) {
			Get();
			Expect(8);
			Expect(9);
			setNodeLabel(); 
		}
		if (la.kind == 11) {
			Get();
			Expect(8);
			Expect(17);
			setNodeRank(); 
		}
		if (la.kind == 12) {
			Get();
			Expect(8);
			Expect(17);
			setNodePosition(); 
		}
		if (la.kind == 14) {
			Get();
			Expect(8);
			Expect(9);
			setNodeCategory(); 
		}
		Expect(6);
	}

	void EDGE() {
		Expect(7);
		Expect(1);
		createEdge(); 
	}

	void EDGE_ATTRS() {
		Color edgeColor; 
		Expect(5);
		if (la.kind == 13) {
			Get();
			Expect(8);
			COLOR(out edgeColor);
			setColor(edgeColor); 
		}
		if (la.kind == 15) {
			Get();
			Expect(8);
			Expect(17);
			setThickness(); 
		}
		if (la.kind == 16) {
			Get();
			Expect(8);
			Expect(17);
			setWeight(); 
		}
		Expect(6);
	}

	void COLOR(out Color color) {
		color = Colors.Black; 
		switch (la.kind) {
		case 22: {
			Get();
			color = Colors.Black; 
			break;
		}
		case 23: {
			Get();
			color = Colors.Blue; 
			break;
		}
		case 24: {
			Get();
			color = Colors.Brown; 
			break;
		}
		case 25: {
			Get();
			color = Colors.Cyan; 
			break;
		}
		case 26: {
			Get();
			color = Colors.DarkGray; 
			break;
		}
		case 27: {
			Get();
			color = Colors.Gray; 
			break;
		}
		case 28: {
			Get();
			color = Colors.Green; 
			break;
		}
		case 29: {
			Get();
			color = Colors.LightGray; 
			break;
		}
		case 30: {
			Get();
			color = Colors.Magenta; 
			break;
		}
		case 31: {
			Get();
			color = Colors.Orange; 
			break;
		}
		case 32: {
			Get();
			color = Colors.Purple; 
			break;
		}
		case 33: {
			Get();
			color = Colors.Red; 
			break;
		}
		case 34: {
			Get();
			color = Colors.Transparent; 
			break;
		}
		case 35: {
			Get();
			color = Colors.White; 
			break;
		}
		case 36: {
			Get();
			color = Colors.Yellow; 
			break;
		}
		case 18: {
			Get();
			color = stringToColor(t.val);	
			break;
		}
		default: SynErr(38); break;
		}
	}



	public void Parse() {
		la = new Token();
		la.val = "";		
		Get();
		GRAPH();
		Expect(0);

	}
	
	static readonly bool[,] set = {
		{T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x}

	};
} // end Parser


public class Errors {
	public int count = 0;                                    // number of errors detected
	public System.IO.TextWriter errorStream = Console.Out;   // error messages go to this stream
  public string errMsgFormat = "-- line {0} col {1}: {2}"; // 0=line, 1=column, 2=text
  
	public virtual void SynErr (int line, int col, int n) {
		string s;
		switch (n) {
			case 0: s = "EOF expected"; break;
			case 1: s = "ident expected"; break;
			case 2: s = "digraph expected"; break;
			case 3: s = "lbrace expected"; break;
			case 4: s = "rbrace expected"; break;
			case 5: s = "lbrack expected"; break;
			case 6: s = "rbrack expected"; break;
			case 7: s = "arrow expected"; break;
			case 8: s = "assgn expected"; break;
			case 9: s = "string expected"; break;
			case 10: s = "label expected"; break;
			case 11: s = "rank expected"; break;
			case 12: s = "order expected"; break;
			case 13: s = "color expected"; break;
			case 14: s = "category expected"; break;
			case 15: s = "thickness expected"; break;
			case 16: s = "weight expected"; break;
			case 17: s = "number expected"; break;
			case 18: s = "argb expected"; break;
			case 19: s = "\"nodes:\" expected"; break;
			case 20: s = "\";\" expected"; break;
			case 21: s = "\"edges:\" expected"; break;
			case 22: s = "\"black\" expected"; break;
			case 23: s = "\"blue\" expected"; break;
			case 24: s = "\"brown\" expected"; break;
			case 25: s = "\"cyan\" expected"; break;
			case 26: s = "\"darkgray\" expected"; break;
			case 27: s = "\"gray\" expected"; break;
			case 28: s = "\"green\" expected"; break;
			case 29: s = "\"lightgray\" expected"; break;
			case 30: s = "\"magenta\" expected"; break;
			case 31: s = "\"orange\" expected"; break;
			case 32: s = "\"purple\" expected"; break;
			case 33: s = "\"red\" expected"; break;
			case 34: s = "\"transparent\" expected"; break;
			case 35: s = "\"white\" expected"; break;
			case 36: s = "\"yellow\" expected"; break;
			case 37: s = "??? expected"; break;
			case 38: s = "invalid COLOR"; break;

			default: s = "error " + n; break;
		}
		errorStream.WriteLine(errMsgFormat, line, col, s);
		count++;
	}

	public virtual void SemErr (int line, int col, string s) {
		errorStream.WriteLine(errMsgFormat, line, col, s);
		count++;
	}
	
	public virtual void SemErr (string s) {
		errorStream.WriteLine(s);
		count++;
	}
	
	public virtual void Warning (int line, int col, string s) {
		errorStream.WriteLine(errMsgFormat, line, col, s);
	}
	
	public virtual void Warning(string s) {
		errorStream.WriteLine(s);
	}
} // Errors


public class FatalError: Exception {
	public FatalError(string m): base(m) {}
}
}