# Perceptron Classification for Accounting

The project implements an averaged perceptron classification algorithm and a use case of the algorithm for posting account classification.

## Build

Use Visual Studio or Visual Studio Code (C#) for compilation.

## Usage

Execute on Windows with `.\PerceptronClassificationForAccounting.exe [Arguments]`
Execute on Linux using `mono` with `mono PerceptronClassificationForAccounting.exe [Arguments]`

Arguments (`T` - for the `train` method, `I` - for `test` (inference) method, `O` -
for the ZeroR/OneR/TwoR method, `S` - for the feature statistics method; `_M` -
mandatory):

| Argument | Description |
| -------- | ----------- |
| `-m [method]` | method - one of `train` (default), `test` (`I_M`), `get-feature-statistics` (`S_M`), `get-one-r-and-zero-r-scores` (`O_M`) |
| `-i [path]` | training data path (`T_M`, `O_M`) or test data path (`I_M`) |
| `-d [path]` | validation data path (`T_M`) or test data (`O_M`) |
| `-o [path]` | output data path (`I_M`) |
| `-mi [path]` | modelInPath (`I_M`, `S_M`) |
| `-mo [path]` | model output path (T_M) |
| `--stem-words` | whether to apply stemming to comment features (`T`, `I`) |
| `-l [language]` | language to use for stemming (`T`, `I`) |
| `--stop-words [path]` | path of the stop-words file (`T`, `I`) |
| `-e [number]` | the maximum number of epochs to run during training (default: `10`) (`T`) |
| `-c [target]` | the classification target to train to predict; one of `debit`, `credit`, or `combined` (default `debit`) (`T_M`, `O_M`) |
| `--use-null` | whether to use the `null` value for unknown/undefined buyers and suppliers instead of the feature `NONE` (i.e., either no feature triggers, or `NONE` triggers) (`T`, `I`) |
| `--skip-amount` | skips amount feature if used (`T`) |
| `--skip-amount-r0` | skips amount rounded to ones if used (`T`) |
| `--skip-amountr-10` | skips amount rounded to tens if used (`T`) |
| `--skip-amount-r100` | skips amount rounded to hundreds if used (`T`) |
| `--skip-doc-amount` | skips document amount feature if used (`T`) |
| `--skip-doc-amount-r0` | skips document amount rounded to ones if used (`T`) |
| `--skip-doc-amountr-10` | skips document amount rounded to tens if used (`T`) |
| `--skip-doc-amount-r100` | skips document amount rounded to hundreds if used (`T`) |
| `--skip-proportion` | skips proportion feature if used (`T`) |
| `--skip-buyer` | skips buyer feature if used (`T`) |
| `--skip-buyer-nace` | skips buyer NACE code feature if used (`T`) |
| `--skip-supplier` | skips supplier feature if used (`T`) |
| `--skip-supplier-nace` | skips supplier NACE code feature if used (`T`) |
| `--skip-year` | skips year feature if used (`T`) |
| `--skip-reverse-vat` | skips reverse VAT indicator feature if used (`T`) |
| `--skip-currency` | skips currency feature if used (`T`) |
| `--skip-comment` | skips row comment features if used (`T`) |
| `--skip-doc-comment` | skips document comment features if used (`T`) |
| `--skip-doc-series` | skips document series feature if used (`T`) |
| `-sn` | skips all numeric features if used (`T`) |
| `-s` | adds three synthetic entries with masked buyers, suppliers, and buyers and suppliers for each training data entry if used (`T`) |
| `-s2` | adds four synthetic entries with masked buyers, suppliers, buyer NACE codes, and supplier NACE codes (requires also -s to be specified, however, it overrides -s behaviour) (`T`) |
| `-a` | instead of the most probable class, prints also all prediction scores of all other classes (`I`) |
| `-mb` | masks buyer if used (`T`, `I`, `O`); this can be used to calculate OneR scores - the TwoR for cases when buyers are masked is equal to OneR. |
| `-ms` | masks supplier if used (`T`, `I`, `O`) |

## Data

Training/validation/test data format: a tab-separated values file (without escaped tabulation symbols without newlines within entries) consisting of the following 14 categories.

| Year | Buyer | Buyer NACE | Supplier | Supplier NACE | Proportion | Doc. Amount | Is Reverse VAT | Debit Account | Credit Account | Row Amount | Currency | Doc. Series | Row comment | Doc. Comment |
| - | - | - | - | - | - | - | - | - | - | - | - | - | - | - |
| 2019 | X | 4719 | X | 4643 | 0.79 | 111.56 | 1 | 7110 | 5310 | 92.2 | EUR | VT | Goods received, sw | Goods received |

Decimal separator for numeric features - period. The TSV file must not feature a header row.

## License

The code is licensed under the MIT license.

## Reference

If you use this code for scientific purposes, please cite the following papers:

```BibTeX
@inproceedings{belskis2020features,
  title={Features and Methods for Automatic Posting Account Classification},
  author={Be{\c{l}}skis, Zigmunds and Zirne, Marita and Pinnis, M{\=a}rcis},
  booktitle={International Baltic Conference on Databases and Information Systems},
  pages={68--81},
  year={2020},
  publisher = {Springer},
}
```

To be published in 2021 (the paper the code was adapted for):
```BibTeX
@article{belskis2021,
  author = {Beļskis, Zigmunds and Zirne, Marita and Slaidiņ{\v{s}}, Viesturs and Pinnis, Mārcis},
  journal = {Baltic Journal of Modern Computing},
  title = {{Natural Language Based Posting Account Classification}},
  year = {2021}
}
```
